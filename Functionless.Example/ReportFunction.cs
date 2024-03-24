using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;

using DurableClientAttribute = Microsoft.Azure.Functions.Worker.DurableClientAttribute;

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

        [Function("reportjob-execute")]
        public async Task Execute(
            [HttpTrigger] HttpRequest request,
            [DurableClient] IDurableClientFactory client)
        {
            await client.DurablyInvokeAsync(
                async () => await this.reportJob.ExecuteAsync()
            );
        }
    }
}
