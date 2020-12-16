Functionless
============

Write More Code, Less Azure Functions

Summary
-------

Functionless is a library to ease your Azure Function development by minimizing the abstraction of your long-running services, processes, workflows, etc.

Serverless platforms like Azure Functions offer the allure of "infinite" on-demand scalability. Combined with durable capabilities via the [Durable Task Framework](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview) and consumption based pricing they also promise efficiency and cost reduction. However, if you've ever tried to create or migrate a long-running process to Azure Functions on a consumption plan you've likely discovered that dividing the workload into orchestrations, activities, entities, queues, etc. can be tedious. If so, Functionless is for you!

Installation
------------

Ensure your Azure Function project targets [Azure Functions v3](https://docs.microsoft.com/en-us/azure/azure-functions/functions-versions) and [Microsoft.NET.Sdk.Functions@3.0.0+](https://www.nuget.org/packages/Microsoft.NET.Sdk.Functions/).

``` XML
<Project Sdk="Microsoft.NET.Sdk">
  ...
  <PropertyGroup>
    ...
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <AzureFunctionsVersion>v3</AzureFunctionsVersion>
    ...
  </PropertyGroup>
    ...
  <ItemGroup>
    ...
    <PackageReference Include="Microsoft.NET.Sdk.Functions" Version="3.0.11" />
    ...
  </ItemGroup>
  ...
</Project>
```

Install [Functionless](https://www.nuget.org/packages/Functionless/) in your Azure Functions v3 project and any dependant projects that need to be called durably.

```
PM> Install-Package Functionless
```

Usage
-----

Add durable attributes to your domain code, here's an example of a simple (albeit useless) report job. ***NOTE**: Durable attributes (i.e. `NewOrchestration`, `SubOrchestration`, `Activity`, `Entity` & `Queue`) must only be applied to methods which are `public`, `virtual` and return a `Task`, else they won't be able to be intercepted and converted into durable function invocations.*

``` C#
public class ReportJob
{
    [NewOrchestration]
    public virtual async Task ExecuteAsync()
    {
        await this.GenerateReportsAsync();
    }

    [SubOrchestration]
    public virtual async Task GenerateReportsAsync()
    {
        await Task.WhenAll(
            Enumerable.Range(0, 1000).Select(_ => this.GenerateReportAsync()).ToArray()
        );
    }

    [Activity]
    public virtual async Task GenerateReportAsync()
    {
        Enumerable.Range(0, 1000000000).Select(p => (long)p).Sum();
    }
}
```

Call your domain code via the built in orchestrator ...

``` HTTP
POST /api/orchestrator?$method=ReportJob.<ExecuteAsync>()
```

Or write via your own function defined trigger ...

``` C#
[FunctionName("ExecuteAsync")]
public async Task ExecuteAsync(
    [HttpTrigger] HttpRequest request,
    [DurableClient] IDurableOrchestrationClient client)
{
    await client.DurablyInvokeAsync(
        async () => await this.reportJob.ExecuteAsync()
    );
}
```

Benefits
--------

* Migrate to Azure Functions without having to manually break your domain code down into endless durable functions.
* Avoid having to write even a single Azure Function via easy to apply attributes using standard async/await patterns you're already familiar with.
* Realize the promise of consummation plan scalability and pricing by easily scaling your domain code up to the maximum number of allowed servers only paying for what you use.
* Compose function chains of orchestrations and activities.
* Spawn new orchestrations without shared histories from existing orchestrations.
* Fan-out/fan-in to broadly execute multiple functions in parallel.
* Support asynchronous workflows awaiting HTTP callbacks at indeterminate times.
* Fire and forget scalable queues of on demand activities.
* Aggregate activities into single addressable entities that must process synchronously one at a time.
* Distribute executions externally to other function app service plans consumption based our otherwise.

Notices
-------

* Functionless requires [Azure Functions v3](https://docs.microsoft.com/en-us/azure/azure-functions/functions-versions) and [Microsoft.NET.Sdk.Functions@3.0.0+](https://www.nuget.org/packages/Microsoft.NET.Sdk.Functions/).
* Functionless depends on [Autofac](https://autofac.org/) for dependency injection and interception of invocations and is extensible.
* Combining or substituting alternative injection techniques will likely have unintended consequences.
* Durable attributes (i.e. `NewOrchestration`, `SubOrchestration`, `Activity`, `Entity` & `Queue`) must only be applied to methods which are `public`, `virtual` and return a `Task`.
* The [Durable Function Code Constraints](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints) must be respected.
* The Microsoft Azure Storage Emulator has some known flaws and can produce unexpected results, especially if using external orchestrations (see [here](https://github.com/Azure/azure-functions-durable-extension/issues/1531)). In such scenarios using an actual Azure Storage Account is recommended.

Troubleshooting
---------------

* [GitHub issues](https://github.com/LeastOne/Functionless/issues) or [Stack Overflow questions](https://stackoverflow.com/questions/tagged/functionless) are welcome, first please be sure to read and understand the ***[Notices](https://github.com/LeastOne/Functionless#Notices)***.
* More examples available via the [Functionless.Example](https://github.com/LeastOne/Functionless/tree/main/Functionless.Example) project in the repository.
* If a custom task-queue name is required you can register your own `INameResolver` to coerce the queue name.

Contributions
-------------

Pull requests welcome!

To-Do's
-------

Here are some needs if you're looking to contribute ...

* TypeService memory cache performance enhancement
* Roslyn analyzer detecting invalid durable attribute usage
* CI Pipelining for build, test & release
* TypeService default parameter matching

Origin
------

The following is a summary of my first journey in attempting to adopt Azure Functions which became the catalyst for Functionless. For illustration purposes I began with the following adaptation of the aforementioned useless `ReportJob` which generated 1,000 reports in a few hours on a 24/7 available server.

``` C#
public class ReportJob
{
    private readonly ILogger logger;

    public ReportJob(ILogger logger)
    {
        this.logger = logger;
    }

    public async Task ExecuteAsync()
    {
        await this.GenerateReportsAsync();
    }

    public async Task GenerateReportsAsync()
    {
        await Task.WhenAll(
            Enumerable.Range(0, 1000).Select(_ => this.GenerateReportAsync()).ToArray()
        );
    }

    public async Task GenerateReportAsync()
    {
        Enumerable.Range(0, 1000000000).Select(p => (long)p).Sum();
    }
    
    public async Task IssueCompleteNotificationAsync()
    {
        this.logger.LogInformation("All Reports Generated");
    }
}
```

I planned to migrate it to an Azure Function on a consumption plan to save cost by only paying for compute cycles when used as opposed to paying for the 24/7 server only used for a few hours a day. Easy enough I thought, I'll create a basic Azure Function app with a simple `HttpTrigger` to kick off the report which looked as follows:

``` C#
public class ReportFunction
{
    private ReportJob reportJob;

    public ReportFunction(ReportJob reportJob)
    {
        this.reportJob = reportJob;
    }

    [FunctionName("ExecuteAsync")]
    public async Task ExecuteAsync([HttpTrigger] HttpRequest request)
    {
        await this.reportJob.ExecuteAsync();
    }
}
```

Piece of cake I thought to myself as I kicked off a request to my function and it began to execute. However, after 5 minutes it failed with a `Timeout value of 00:05:00 exceeded by function` exception at which point I discovered that consumption plans can only execute a single activity for a maximum of 10 minutes (5 minutes by default). Researching further I realize that the Durable Task Framework is designed for exactly this purpose to divide a workload into a series of orchestrations and activities. No problem I thought to myself, I'll just execute an orchestration with each report generation as an activity. A bit later I had a `ReportFunction` that looked something like the following:

``` C#
public class ReportFunction
{
    private ReportJob reportJob;

    public ReportFunction(ReportJob reportJob)
    {
        this.reportJob = reportJob;
    }

    [FunctionName("ExecuteAsync")]
    public async Task ExecuteAsync(
        [HttpTrigger] HttpRequest request,
        [DurableClient] IDurableOrchestrationClient client)
    {
        await client.StartNewAsync("GenerateReportsAsync");
    }

    [FunctionName("GenerateReportsAsync")]
    public async Task GenerateReportsAsync(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        await Task.WhenAll(
            Enumerable.Range(0, 1000).Select(
                _ => context.CallActivityAsync("GenerateReportAsync", default)
            ).ToArray()
        );

        await this.reportJob.IssueCompleteNotificationAsync();
    }

    [FunctionName("GenerateReportAsync")]
    public async Task GenerateReportAsync(
        [ActivityTrigger] IDurableActivityContext context)
    {
        await this.reportJob.GenerateReportAsync();
    }
}
```

Its at about this time I realize that this approach is starting to feel a bit unmanageable. I've now had to create three functions in `ReportFunction` to call just two functions in the `ReportJob`. In addition, I realize I also had to move some of the business logic into the `GenerateReports` function in order to fan-out the `GenerateReport` activity calls which means the implementation of my `ReportJob` is now dependent upon details in the `ReportFunction` and Azure Functions in general.

Setting that aside momentarily I decide to run my job anyway. Which works rather well, thanks to the orchestration and activities on the consumption plan the job starts up a few dozen servers and nears completion after several minutes at a fraction of the cost. Just as I'm about to pat myself on the back as I watch the execution near completion a `Multithreaded execution was detected` exception is thrown. More research reveals that although the `ReportJob.IssueCompleteNotification` method executes in under 5 minutes, durable functions can only await other durable functions. Feeling somewhat discouraged I churn out another function ending up with the following.

``` C#
public class ReportFunction
{
    private ReportJob reportJob;

    public ReportFunction(ReportJob reportJob)
    {
        this.reportJob = reportJob;
    }

    [FunctionName("ExecuteAsync")]
    public async Task ExecuteAsync(
        [HttpTrigger] HttpRequest request,
        [DurableClient] IDurableOrchestrationClient client)
    {
        await client.StartNewAsync("GenerateReportsAsync");
    }

    [FunctionName("GenerateReportsAsync")]
    public async Task GenerateReportsAsync(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        await Task.WhenAll(
            Enumerable.Range(0, 1000).Select(
                _ => context.CallActivityAsync("GenerateReportAsync", default)
            ).ToArray()
        );

        await context.CallActivityAsync("IssueCompleteNotificationAsync", default);
    }

    [FunctionName("GenerateReportAsync")]
    public async Task GenerateReportAsync(
        [ActivityTrigger] IDurableActivityContext context)
    {
        await this.reportJob.GenerateReportAsync();
    }

    [FunctionName("IssueCompleteNotificationAsync")]
    public async Task IssueCompleteNotificationAsync(
        [ActivityTrigger] IDurableActivityContext context)
    {
        await this.reportJob.IssueCompleteNotificationAsync();
    }
}
```

After successfully re-executing the job all the way to completion I'm pleased to have achieved the sought after scalability, performance and cost savings. However, reviewing the necessary added code which now contains intermixed host and domain logic I'm left with serious doubts about the feasibility of applying the approach to other more complex scenarios. Those doubts became the catalyst to seek out an alternative method which resulted in the creation of Functionless.