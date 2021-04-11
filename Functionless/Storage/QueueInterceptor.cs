using System;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

using Castle.DynamicProxy;

using Functionless.Durability;
using Functionless.Injection;
using Functionless.Reflection;

namespace Functionless.Storage
{
    [Precedence(2)]
    public class QueueInterceptor : IInterceptor
    {
        public static AsyncLocal<bool> Execute { get; } = new AsyncLocal<bool>();

        private IConfiguration configuration;

        private Lazy<TypeService> typeSerivce;

        public QueueInterceptor(IConfiguration configuration, Lazy<TypeService> typeSerivce)
        {
            this.configuration = configuration;
            this.typeSerivce = typeSerivce;
        }

        public virtual void Intercept(IInvocation invocation)
        {
            var queueAttribute = Attribute
                .GetCustomAttributes(invocation.Method, typeof(QueueAttribute))
                .Cast<QueueAttribute>()
                .FirstOrDefault();

            var shouldEnqueue = queueAttribute != null && !Execute.Value;

            if (shouldEnqueue)
            {
                invocation.ReturnValue = this.Enqueue(invocation);
            }
            else
            {
                invocation.Proceed();
            }
        }

        protected virtual dynamic Enqueue(IInvocation invocation)
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(
                this.configuration.GetValue<string>("AzureWebJobsStorage")
            );

            // Create the queue client.
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            var queue = queueClient.GetQueueReference(
                (this.configuration.GetValue<string>("TaskQueue") ?? "TaskQueue").ToLower()
            );

            // Create the queue if it doesn't already exist.
            return queue.CreateIfNotExistsAsync().ContinueWith(
                _ => queue.AddMessageAsync(
                    new CloudQueueMessage(
                        new FunctionContext {
                            MethodSpecification = this.typeSerivce.Value.GetMethodSpecification(invocation.Method),
                            Arguments = invocation.Method.GetParameters().Zip(invocation.Arguments, (a, b) => (a.Name, b)).ToDictionary()
                        }.ToJson()
                    )
                )
            );
        }
    }
}
