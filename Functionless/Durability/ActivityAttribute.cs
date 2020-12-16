using System.Threading.Tasks;

namespace Functionless.Durability
{
    public class ActivityAttribute : DurableAttribute
    {
        public ActivityAttribute(string externalOrchestratorUrlOrAppSetting = null)
            : base(1, externalOrchestratorUrlOrAppSetting) { }

        public override async Task<TResult> Invoke<TResult>(DurableContext context)
        {
            return await context.OrchestrationContext.CallActivityAsync<TResult>(
                context.FunctionContext.FunctionName,
                context.FunctionContext
            );
        }
    }
}
