using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functionless.Durability
{
    public class DurableContext
    {
        public IDurableOrchestrationContext OrchestrationContext { get; set; }

        public IDurableOrchestrationClient OrchestrationClient { get; set; }

        public FunctionContext FunctionContext { get; set; }
    }
}
