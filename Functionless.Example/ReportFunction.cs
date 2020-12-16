using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

using Functionless.Durability;

namespace Functionless.Example
{
    public class ReportFunction
    {
        private readonly ReportJob reportJob;

        public ReportFunction(ReportJob reportJob)
        {
            this.reportJob = reportJob;
        }

        [FunctionName("reportjob-execute")]
        public async Task Execute(
            [HttpTrigger] HttpRequest request,
            [DurableClient] IDurableOrchestrationClient client)
        {
            await client.DurablyInvokeAsync(
                async () => await this.reportJob.ExecuteAsync()
            );
        }
    }
}
