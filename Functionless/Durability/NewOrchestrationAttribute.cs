using System.Threading.Tasks;

namespace Functionless.Durability
{
    public class NewOrchestrationAttribute : OrchestrationAttribute
    {
        public NewOrchestrationAttribute(string externalOrchestratorUrlOrAppSetting = null)
            : base(externalOrchestratorUrlOrAppSetting) { }

        public NewOrchestrationAttribute(bool isSingleInstance, string externalOrchestratorUrlOrAppSetting = null)
            : base(isSingleInstance, externalOrchestratorUrlOrAppSetting) { }

        public NewOrchestrationAttribute(string instanceId, string externalOrchestratorUrlOrAppSetting = null)
            : base(instanceId, externalOrchestratorUrlOrAppSetting) { }

        public override Task<TResult> Invoke<TResult>(DurableContext context)
        {
            if (context.OrchestrationContext != null)
            {
                context.OrchestrationContext.StartNewOrchestration(
                    context.FunctionContext.FunctionName,
                    context.FunctionContext,
                    context.FunctionContext.InstanceId
                );
            }
            else
            {
                context.OrchestrationClient.StartNewAsync(
                    "orchestration",
                    context.FunctionContext.InstanceId,
                    context.FunctionContext
                );
            }

            return Task.FromResult(default(TResult));
        }
    }
}
