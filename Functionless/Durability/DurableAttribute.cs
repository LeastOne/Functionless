using System;
using System.Threading.Tasks;

namespace Functionless.Durability
{
    public abstract class DurableAttribute : Attribute
    {
        public DurableAttribute(int preferredExecutionPriority, string externalOrchestratorUrlOrAppSetting = null)
        {
            this.PreferredExecutionPriority = preferredExecutionPriority;
            this.ExternalOrchestratorUrlOrAppSetting = externalOrchestratorUrlOrAppSetting;
        }

        public int PreferredExecutionPriority { get; private set; }

        public string ExternalOrchestratorUrlOrAppSetting { get; private set; }

        public bool IsSingleInstance { get; protected set; }

        public string InstanceId { get; protected set; }

        public virtual string GetFunctionName()
        {
            return this.GetType().Name.Replace("Attribute", string.Empty).ToLower();
        }

        public abstract Task<TResult> Invoke<TResult>(DurableContext context);
    }
}
