using System.Threading.Tasks;

namespace Functionless.Durability
{
    public class SubOrchestrationAttribute : OrchestrationAttribute
    {
        public SubOrchestrationAttribute(string externalOrchestratorUrlOrAppSetting = null)
            : base(externalOrchestratorUrlOrAppSetting) { }

        public SubOrchestrationAttribute(bool isSingleInstance, string externalOrchestratorUrlOrAppSetting = null)
            : base(isSingleInstance, externalOrchestratorUrlOrAppSetting) { }

        public SubOrchestrationAttribute(string instanceId, string externalOrchestratorUrlOrAppSetting = null)
            : base(instanceId, externalOrchestratorUrlOrAppSetting) { }

        public override async Task<TResult> Invoke<TResult>(DurableContext context)
        {
            return await context.OrchestrationContext.CallSubOrchestratorAsync<TResult>(
                context.FunctionContext.FunctionName,
                context.FunctionContext.InstanceId,
                context.FunctionContext
            );
        }
    }
}
