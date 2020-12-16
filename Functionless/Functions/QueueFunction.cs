using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;

using Autofac;

using Functionless.Durability;
using Functionless.Storage;

namespace Functionless.Functions
{
    public class QueueFunction
    {
        private IComponentContext componentContext;

        public QueueFunction(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        [FunctionName("queue")]
        public async Task QueueAync([QueueTrigger("TaskQueue")] FunctionContext functionContext)
        {
            QueueInterceptor.Execute.Value = true;
            await functionContext.InvokeAsync(componentContext);
        }
    }
}
