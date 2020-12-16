using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Primitives;

namespace Functionless.Durability
{
    public static class DurableExtensions
    {
        public static async Task<IEnumerable<DurableOrchestrationStatus>> GetInstancesAsync(this IDurableOrchestrationClient durableOrchestrationClient)
        {
            var instances = await durableOrchestrationClient.ListInstancesAsync(
                new OrchestrationStatusQueryCondition(),
                CancellationToken.None
            );

            return instances.DurableOrchestrationState;
        }

        public static async Task DurablyInvokeAsync(this IDurableOrchestrationClient durableOrchestrationClient, Func<Task> func)
        {
            DurableInterceptor.Context.Value =
                new DurableContext {
                    OrchestrationClient = durableOrchestrationClient
                };

            await func();
        }

        public static async Task IssueCallbackIfNecessary(this IDurableOrchestrationContext durableOrchestrationContext, FunctionContext functionContext, object content)
        {
            if (functionContext.CallbackUrl.IsNullOrWhiteSpace()) return;

            // NOTE: The Azure Storage Emulator is flaky and when used its possible for the callback to be issued
            // before the instance-id callback handler is ready. When it does a 404 not-found is returned. Thereofore,
            // in that event we'll continue to retry until successful. Its been confirmed that this is not an issue
            // when using an Azure Storage Account see the following issue for more details.
            // https://github.com/Azure/azure-functions-durable-extension/issues/1531

            // NOTE: Regarding `asynchronousPatternEnabled`, disabling it is important because when enabled it attempts
            // to apply an asynchronous pattern based on HTTP Accepted responses (status code 202) which coincidentally
            // is exactly what the Durable Functionsbuilt-in HTTP APIs return when acknowledging submitted events and
            // therefore fools the `CallHttpAsync` method into trying to apply the asynchronous pattern.

            DurableHttpResponse response;

            do
            {
                response = await durableOrchestrationContext.CallHttpAsync(
                    new DurableHttpRequest(
                        HttpMethod.Post,
                        new Uri(functionContext.CallbackUrl),
                        new Dictionary<string, StringValues>() { { "Content-Type", "application/json" } },
                        content.ToJson(),
                        asynchronousPatternEnabled: false
                    )
                );
            }
            while (response.StatusCode == HttpStatusCode.NotFound);

        }
    }
}
