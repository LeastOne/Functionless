Functionless.Example
====================

Summary
-------

The `Functionless.Example` project demonstrates the core concepts of Functionless.

Execution
---------

Invoke the `ReportJob` ...

1. Via the built in Functionless orchestrator ...

``` HTTP
POST /api/orchestrator?$method=ReportJob.<ExecuteAsync>()
```

2. Via the custom HttpTrigger in `ReportFunction` ...

``` HTTP
POST /api/reportjob-execute
```

Modify the report method, count and/or load ...

1. Via the configuration in `appsettings.json` ...

```JSON
"ReportConfig": {
  "ExternalOrchestratorUrl": "http://localhost:7071/api/orchestrator",
  "ReportMethod": "Entity",
  "ReportCount": 10,
  "ReportLoad": 1000000
}
```

2. Via the built in Functionless orchestrator ...

``` HTTP
POST /api/orchestrator

{
    "MethodSpecification": "Functionless.Example.ReportJob.<GenerateReportsAsync>(Nullable`1[ReportMethod], Nullable`1[Int32], Nullable`1[Int32])",
    "Arguments": {
        "ReportMethod": "Entity",
        "ReportCount": "10",
        "ReportLoad": "1000000"
    },
    "Await": false
}
```