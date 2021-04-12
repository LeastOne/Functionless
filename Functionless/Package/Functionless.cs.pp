using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof($rootnamespace$.Functionless))]

namespace $rootnamespace$
{
    public partial class Functionless : global::Functionless.Injection.Startup { }
}
