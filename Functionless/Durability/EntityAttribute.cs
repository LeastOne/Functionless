using System.Threading.Tasks;

using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functionless.Durability
{
    public class EntityAttribute : DurableAttribute
    {
        public EntityAttribute(string externalOrchestratorUrlOrAppSetting = null)
            : base(1, externalOrchestratorUrlOrAppSetting)
        {
            this.IsSingleInstance = true;
        }

        public EntityAttribute(string instanceId, string externalOrchestratorUrlOrAppSetting = null)
            : this(externalOrchestratorUrlOrAppSetting)
        {
            this.InstanceId = instanceId;
        }

        public override async Task<TResult> Invoke<TResult>(DurableContext context)
        {
            return await context.OrchestrationContext.CallEntityAsync<TResult>(
                new EntityId(
                    context.FunctionContext.FunctionName,
                    context.FunctionContext.InstanceId
                ),
                null,
                context.FunctionContext
            );
        }
    }
}
