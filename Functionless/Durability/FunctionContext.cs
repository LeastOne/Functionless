using System;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using Newtonsoft.Json.Linq;

using Functionless.Reflection;

namespace Functionless.Durability
{
    public class FunctionContext
    {
        public string BaseUrl { get; set; }

        public string FunctionName { get; set; }

        public string InstanceId { get; set; }

        public string MethodSpecification { get; set; }

        public (string Name, object Value)[] Arguments { get; set; }

        public bool Await { get; set; }

        public string CallbackUrl { get; set; }

        public async Task<object> InvokeAsync(IComponentContext componentContext)
        {
            try
            {
                var typeService = componentContext.Resolve<TypeService>();
                var method = typeService.GetMethod(this.MethodSpecification);
                var parameters = method.GetParameters();
                var instance = componentContext.Resolve(method.DeclaringType);
                var arguments = (
                    from p in parameters
                    join a in this.Arguments on p.Name equals a.Name
                    select a.Value is JToken
                        ? (a.Value as JToken).ToObject(p.ParameterType)
                        : a.Value.ChangeType(p.ParameterType)
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
