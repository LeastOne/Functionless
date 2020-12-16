namespace Functionless.Example
{
    public interface IConfig { }

    public enum ReportMethod
    {
        Activity, ActivityExternal, Entity, Queue
    }

    public class ReportConfig : IConfig
    {
        public string ExternalOrchestratorUrl { get; set; }

        public ReportMethod ReportMethod { get; set; }

        public int ReportCount { get; set; }

        public int ReportLoad { get; set; }
    }
}
