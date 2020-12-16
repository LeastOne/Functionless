using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Functionless.Durability;
using Functionless.Storage;

namespace Functionless.Example
{
    public class ReportJob
    {
        private readonly ReportConfig reportConfig;

        private readonly ILogger logger;

        public ReportJob(ReportConfig reportConfig, ILogger logger)
        {
            this.reportConfig = reportConfig;
            this.logger = logger;
        }

        [NewOrchestration]
        public virtual async Task ExecuteAsync(ReportMethod? reportMethod = null, int? reportCount = null, int? reportLoad = null)
        {
            await this.GenerateReportsAsync(reportMethod);
        }

        [SubOrchestration]
        public virtual async Task GenerateReportsAsync(ReportMethod? reportMethod = null, int? reportCount = null, int? reportLoad = null)
        {
            reportMethod = reportMethod ?? this.reportConfig.ReportMethod;

            var method = this.GetType().GetMethod(
                $"GenerateReport{reportMethod}Async"
            );

            var tasks = Enumerable.Range(0, this.reportConfig.ReportCount).Select(
                _ => method.Invoke(this, null) as Task
            ).ToArray();

            if (reportMethod != ReportMethod.Queue)
            {
                await Task.WhenAll(tasks);
            }

            await this.IssueCompleteNotificationAsync();
        }

        [Activity]
        public virtual async Task GenerateReportActivityAsync()
        {
            await this.GenerateReportAsync();
        }

        [Activity("ReportConfig:ExternalOrchestratorUrl")]
        public virtual async Task GenerateReportActivityExternalAsync()
        {
            await this.GenerateReportAsync();
        }

        [Entity]
        public virtual async Task GenerateReportEntityAsync()
        {
            await this.GenerateReportAsync();
        }

        [Queue]
        public virtual async Task GenerateReportQueueAsync()
        {
            await this.GenerateReportAsync();
        }

        [Activity]
        public virtual Task IssueCompleteNotificationAsync()
        {
            this.logger.LogInformation("All Reports Generated");

            return Task.CompletedTask;
        }

        private Task GenerateReportAsync()
        {
            Enumerable.Range(0, Environment.ProcessorCount).AsParallel().Select(
                _ => Enumerable.Range(1, this.reportConfig.ReportLoad).Select(p => (long)p).Sum()
            ).Last();

            return Task.CompletedTask;
        }
    }
}
