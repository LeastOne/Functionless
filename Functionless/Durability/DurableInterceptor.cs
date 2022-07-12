using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Autofac;

using Castle.DynamicProxy;

using Functionless.Injection;
using Functionless.Reflection;

namespace Functionless.Durability
{
    [Precedence(1)]
    public class DurableInterceptor : IInterceptor
    {
        private static AsyncLocal<int> Depth { get; } = new AsyncLocal<int>();

        private Lazy<TypeService> typeSerivce;

        private IConfiguration configuration;

        public DurableInterceptor(Lazy<TypeService> typeSerivce, IConfiguration configuration)
        {
            this.typeSerivce = typeSerivce;
            this.configuration = configuration;
        }

        public static AsyncLocal<DurableContext> Context { get; } = new AsyncLocal<DurableContext>();

        public void Intercept(IInvocation invocation)
        {
            Depth.Value++;

            var durableAttribute = Attribute
                .GetCustomAttributes(invocation.Method, typeof(DurableAttribute))
                .Cast<DurableAttribute>()
                .OrderBy(ks => Depth.Value == 1 ? ks.PreferredExecutionPriority : 0)
                .ThenByDescending(ks => Depth.Value != 1 ? ks.PreferredExecutionPriority : 0)
                .FirstOrDefault();

            var shouldInvokeDurably =
                Context.Value != null &&
                durableAttribute != null && (
                    Depth.Value > 1 ||
                    durableAttribute.GetType().IsAssignableTo<ActivityAttribute>() ||
                    (durableAttribute.GetType().IsAssignableTo<NewOrchestrationAttribute>() && Context.Value.OrchestrationContext == null)
                );

            if (shouldInvokeDurably)
            {
                invocation.ReturnValue =
                    this.GetType()
                        .GetMethod(nameof(DurablyInvoke), BindingFlags.Instance | BindingFlags.NonPublic)
                        .TryMakeGenericMethod(
                            invocation.Method.ReturnType?.GenericTypeArguments?.Any() == true ?
                                invocation.Method.ReturnType.GenericTypeArguments :
                                new[] { typeof(object) }
                        ).Invoke(this, new object[] { durableAttribute, invocation });
            }
            else
            {
                invocation.Proceed();
            }

            Depth.Value--;
        }

        protected Task<TResult> DurablyInvoke<TResult>(DurableAttribute durableAttribute, IInvocation invocation)
        {
            if (!invocation.Method.ReturnType.IsAssignableTo<Task>())
            {
                throw new DurableInvocationException(
                    $"Method {invocation.Method} has a return type of {invocation.Method.ReturnType} but its application of {durableAttribute.GetType().Name} requries its return type be of type {typeof(Task)}."
                );
            }

            var baseUrl = Context.Value.FunctionContext?.BaseUrl;
            var functionName = durableAttribute.GetFunctionName();
            var methodSpecificaiton = this.typeSerivce.Value.GetMethodSpecification(invocation.TargetType, invocation.Method);
            var instanceId = durableAttribute.IsSingleInstance ? durableAttribute.InstanceId ?? methodSpecificaiton : null;
            var methodArguments = invocation.Method.GetParameters().Zip(invocation.Arguments, (a, b) => (a.Name, Value: b)).ToDictionary();

            var durableContext = new DurableContext {
                OrchestrationContext = Context.Value.OrchestrationContext,
                OrchestrationClient = Context.Value.OrchestrationClient,
                FunctionContext = new FunctionContext {
                    BaseUrl = baseUrl,
                    FunctionName = functionName,
                    InstanceId = instanceId,
                    MethodSpecification = methodSpecificaiton,
                    Instance = invocation.InvocationTarget,
                    Arguments = methodArguments
                }
            };

            return Context.Value?.FunctionContext?.MethodSpecification == methodSpecificaiton ||
                durableAttribute.ExternalOrchestratorUrlOrAppSetting.IsNullOrWhiteSpace() ?
                this.DurablyInvokeInternally<TResult>(durableAttribute, durableContext) :
                this.DurablyInvokeExternally<TResult>(durableAttribute, durableContext);
        }

        protected async Task<TResult> DurablyInvokeInternally<TResult>(DurableAttribute durableAttribute, DurableContext durableContext)
        {
            return await durableAttribute.Invoke<TResult>(durableContext);
        }

        protected async Task<TResult> DurablyInvokeExternally<TResult>(DurableAttribute durableAttribute, DurableContext durableContext)
        {
            const string eventName = "external-durable-invocation-complete";
            durableContext.FunctionContext.CallbackUrl = durableContext.OrchestrationClient.CreateHttpManagementPayload(durableContext.OrchestrationContext.InstanceId).SendEventPostUri.Replace("{eventName}", eventName);
            var url = this.configuration.GetValue<string>(durableAttribute.ExternalOrchestratorUrlOrAppSetting) ?? durableAttribute.ExternalOrchestratorUrlOrAppSetting;

            await durableContext.OrchestrationContext.CallHttpAsync(
                HttpMethod.Post,
                new Uri(url),
                durableContext.FunctionContext.ToJson()
            );

            return await durableContext.OrchestrationContext.WaitForExternalEvent<TResult>(eventName);
        }
    }
}
