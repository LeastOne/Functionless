using Microsoft.Azure.Functions.Extensions.DependencyInjection;

using FunctionlessStartup = Functionless.Startup;

[assembly: FunctionsStartup(typeof($rootnamespace$.Functionless))]

namespace $rootnamespace$
{
    public class Functionless : FunctionlessStartup
    {
        public Functionless() : base(typeof(Functionless).Assembly) { }
    }
}
