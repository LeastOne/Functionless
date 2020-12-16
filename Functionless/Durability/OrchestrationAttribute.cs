namespace Functionless.Durability
{
    public abstract class OrchestrationAttribute : DurableAttribute
    {
        protected OrchestrationAttribute(string externalOrchestratorUrlOrAppSetting = null)
            : base(2, externalOrchestratorUrlOrAppSetting) { }

        public OrchestrationAttribute(bool isSingleInstance, string externalOrchestratorUrlOrAppSetting = null)
            : this(externalOrchestratorUrlOrAppSetting)
        {
            this.IsSingleInstance = isSingleInstance;
        }

        public OrchestrationAttribute(string instanceId, string externalOrchestratorUrlOrAppSetting = null)
            : this()
        {
            this.IsSingleInstance = true;
            this.InstanceId = instanceId;
        }

        public override string GetFunctionName()
        {
            return nameof(OrchestrationAttribute).Replace("Attribute", string.Empty).ToLower();
        }
    }
}
