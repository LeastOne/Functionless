using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

using Autofac;

using Functionless.Durability;

namespace Functionless.Functions
{
    public class DurableFunction
    {
        private IComponentContext componentContext;

        public DurableFunction(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        [FunctionName("orchestration")]
        public async Task<object> OrchestrationAsync(
            [OrchestrationTrigger] IDurableOrchestrationContext durableOrchestrationContext,
            [DurableClient] IDurableOrchestrationClient durableOrchestrationClient)
        {
            var functionContext = durableOrchestrationContext.GetInput<FunctionContext>();
            DurableInterceptor.Context.Value = new DurableContext { OrchestrationContext = durableOrchestrationContext, OrchestrationClient = durableOrchestrationClient, FunctionContext = functionContext };
            var result = await functionContext.InvokeAsync(this.componentContext);
            await durableOrchestrationContext.IssueCallbackIfNecessary(functionContext, result);
            return result;
        }

        [FunctionName("activity")]
        public async Task<object> ActivityAsync([ActivityTrigger] FunctionContext functionContext)
        {
            DurableInterceptor.Context.Value = default;
            return await functionContext.InvokeAsync(this.componentContext);
        }

        [FunctionName(name: "entity")]
        public async Task EntityAsync([EntityTrigger] IDurableEntityContext durableEntityContext)
        {
            DurableInterceptor.Context.Value = default;
            var functionContext = durableEntityContext.GetInput<FunctionContext>();
            var result = await functionContext.InvokeAsync(this.componentContext);
            durableEntityContext.DeleteState();
            durableEntityContext.Return(result);
        }
    }
}
