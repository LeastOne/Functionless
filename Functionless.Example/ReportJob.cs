using System;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
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
        public virtual async Task ExecuteAsync(ReportConfig reportConfig = null)
        {
            logger.LogWarning("Default ReportConfig");
            logger.LogWarning(JsonConvert.SerializeObject(this.reportConfig, Formatting.Indented));

            if (reportConfig != null)
            {
                logger.LogWarning("Received ReportConfig");
                var input = JsonConvert.SerializeObject(reportConfig, Formatting.Indented);
                logger.LogWarning(input);

                JsonConvert.PopulateObject(input, this.reportConfig);

                logger.LogWarning("Resulting ReportConfig");
                logger.LogWarning(JsonConvert.SerializeObject(this.reportConfig, Formatting.Indented));
            }

            //await this.GenerateReportsAsync();
        }

        [SubOrchestration]
        public virtual async Task GenerateReportsAsync(ReportMethod? reportMethod = null, int? reportCount = null, int? reportLoad = null)
        {
            this.reportConfig.ReportMethod = reportMethod ?? this.reportConfig.ReportMethod;
            this.reportConfig.ReportCount = reportCount ?? this.reportConfig.ReportCount;
            this.reportConfig.ReportLoad = reportLoad ?? this.reportConfig.ReportLoad;

            var method = this.GetType().GetMethod(
                $"GenerateReport{this.reportConfig.ReportMethod}Async"
            );

            var tasks = Enumerable.Range(0, this.reportConfig.ReportCount).Select(
                _ => method.Invoke(this, null) as Task
            ).ToArray();

            if (this.reportConfig.ReportMethod != ReportMethod.Queue)
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
            this.logger.LogWarning("Report Generated");

            Enumerable.Range(0, Environment.ProcessorCount).AsParallel().Select(
                _ => Enumerable.Range(1, this.reportConfig.ReportLoad).Select(p => (long)p).Sum()
            ).Last();

            return Task.CompletedTask;
        }
    }
}
