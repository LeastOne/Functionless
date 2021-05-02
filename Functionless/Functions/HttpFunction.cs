using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

using Autofac;

using Newtonsoft.Json.Linq;

using Functionless.Durability;

namespace Functionless.Functions
{
    public class HttpFunction
    {
        private IComponentContext componentContext;

        public HttpFunction(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        [FunctionName("orchestrator")]
        public async Task<object> OrchestratorAsync(
            [HttpTrigger("get", "post", "delete")] HttpRequest request,
            [DurableClient] IDurableOrchestrationClient durableOrchestrationClient)
        {
            var method = request.Method.ToLower();

            if (method == "get")
            {
                return await durableOrchestrationClient.GetInstancesAsync();
            }

            if (method == "delete")
            {
                await Task.WhenAll(
                    from i in await durableOrchestrationClient.GetInstancesAsync()
                    where i.RuntimeStatus == OrchestrationRuntimeStatus.Running
                    select durableOrchestrationClient.TerminateAsync(i.InstanceId, "terminate-all")
                );

                return await Task.WhenAll(
                    from i in await durableOrchestrationClient.GetInstancesAsync()
                    where i.RuntimeStatus != OrchestrationRuntimeStatus.Running
                    select durableOrchestrationClient.PurgeInstanceHistoryAsync(i.InstanceId)
                );
            }

            var body = await request.Body.ReadAsync();
            var functionContext = body.FromJson<FunctionContext>() ?? new FunctionContext();
            functionContext.BaseUrl = new Uri(request.GetDisplayUrl()).GetLeftPart(UriPartial.Authority);
            functionContext.MethodSpecification = functionContext.MethodSpecification ?? request.Query["$method"];
            functionContext.Await = functionContext.Await || ((string)request.Query["$await"]).ChangeType<bool>();
            functionContext.CallbackUrl = functionContext.CallbackUrl ?? request.Query["$callbackUrl"];
            functionContext.Arguments = functionContext.Arguments ?? JToken.FromObject((
                from q in request.Query
                where !q.Key.StartsWith("$")
                select (q.Key, Value: q.Value.FirstOrDefault() as object)
            ).ToDictionary());

            if (functionContext.Await)
            {
                return await functionContext.InvokeAsync(this.componentContext);
            }

            var instanceId = await durableOrchestrationClient.StartNewAsync("orchestration", functionContext);

            return durableOrchestrationClient.CreateHttpManagementPayload(instanceId) as object;
        }
    }
}
