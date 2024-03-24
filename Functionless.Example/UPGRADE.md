UPGRADE
=======

On 3/24/2024 an upgrade of dependencies was undertaken which led to the discovery that Microsoft has introduced a new Isolated mode in Azure Functions as opposed to In-Process. It seems that Microsoft's longterm plan is to shift permanently to Isolated mode. Therefore its necessary to eventually make this jump. In this commit I started the upgrade process, its possible that it actually might work. However, I suspect there are issues with the `Functionless.Injection.Startp` instance. Ultimately I think this will take a bit more energy than I want to exert at the moment and thus am going to put it off until some other priorities can be addressed.

Notes
-----

- The existing In-Process solution will eventually be upgraded from .NET 6 to .NET 8, but it will be the last upgrade for In-Process
- Beyond .NET 8 the Isolated mode will be the only way to use Azure Functions, LTS for .NET 8 ends 11/10/2026
- Generally speaking the Autofac approach used should still work, just needs to be reworked a bit
    - Hoping that the `Autofac.Extensions.DependencyInjection.AzureFunctions` will be upgraded to further help

Resources
---------

- [Guide for running C# Azure Functions in an isolated worker process](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide)
- [Migrate .NET apps from the in-process model to the isolated worker model](https://learn.microsoft.com/en-us/azure/azure-functions/migrate-dotnet-to-isolated-model)
- [Differences between the isolated worker model and the in-process model for .NET on Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-in-process-differences)
- [.NET on Azure Functions – August 2023 roadmap update](https://techcommunity.microsoft.com/t5/apps-on-azure-blog/net-on-azure-functions-august-2023-roadmap-update/ba-p/3910098)
- [Develop C# class library functions using Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library)
- [AZFW0001: Invalid binding attributes](https://learn.microsoft.com/en-us/azure/azure-functions/errors-diagnostics/net-worker-rules/azfw0001)