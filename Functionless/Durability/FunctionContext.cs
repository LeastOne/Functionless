using System;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using Newtonsoft.Json.Linq;

using Functionless.Json;
using Functionless.Reflection;

namespace Functionless.Durability
{
    public class FunctionContext
    {
        public string BaseUrl { get; set; }

        public string FunctionName { get; set; }

        public string InstanceId { get; set; }

        public string MethodSpecification { get; set; }

        public dynamic Instance { get; set; }

        public dynamic Arguments { get; set; }

        public bool Await { get; set; }

        public string CallbackUrl { get; set; }

        public async Task<object> InvokeAsync(IComponentContext componentContext)
        {
            try
            {
                var typeService = componentContext.Resolve<TypeService>();
                var method = typeService.GetMethod(this.MethodSpecification);
                var parameters = method.GetParameters();
                var reflectedTypeFullName = method.ReflectedType.FullName;
                var instance = componentContext.Resolve(method.ReflectedType);
                if (this.Instance != null)
                    using (var reader = (this.Instance as JToken).CreateReader())
                        Serializer.Default.Populate(reader, instance);
                var arguments = (
                    from p in parameters
                    join a in this.Arguments as JToken on p.Name.ToLower() equals a.Path.ToLower()
                    select (a as JProperty).Value.ToObject(p.ParameterType, Serializer.Default)
                ).ToArray();
                var task = method.Invoke(instance, arguments) as dynamic; await task;
                return method.ReturnType.IsGenericType ? task.Result : null;
            }
            catch (Exception e)
            {
                // The durable task framework chokes on excpetions that aren't serializeable, the following is a hack
                // to bypass that problem. In lieu of Microsoft providing a better solution this could be made smarter
                // to try serializing and then deserializing to ensure the exception can go both ways, perhaps even
                // caching the serializability of an exception type so as not to continuously keep reperforming the
                // same checks.
                throw new Exception(e.ToJson(false));
            }
        }
    }
}
