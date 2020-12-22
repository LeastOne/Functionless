Functionless.Example
====================

Summary
-------

The `Functionless.Example` project demonstrates the core concepts of Functionless.

Execution
---------

The following approaches can be used to invoke the `ReportJob` ...

1. Via the built in Functionless orchestrator ...

``` HTTP
POST /api/orchestrator?$method=ReportJob.<ExecuteAsync>()
```

2. Via the custom HttpTrigger in `ReportFunction` ...

``` HTTP
POST /api/reportjob-execute
```
