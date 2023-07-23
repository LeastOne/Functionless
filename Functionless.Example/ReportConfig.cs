namespace Functionless.Example
{
    public interface IConfig { }

    public enum ReportMethod
    {
        Activity, ActivityExternal, Entity, Queue
    }

    public class ReportConfig : IConfig
    {
        public string ExternalOrchestratorUrl { get; set; } = "http://localhost:7071/api/orchestrator";

        public ReportMethod ReportMethod { get; set; } = ReportMethod.Activity;

        public int ReportCount { get; set; } = 10;

        public int ReportLoad { get; set; } = 1000;
    }
}
