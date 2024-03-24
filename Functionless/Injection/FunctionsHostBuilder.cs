using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Functionless.Injection
{
    public class FunctionsHostBuilder : IFunctionsHostBuilder
    {
        public IServiceCollection Services { get; set; }
    }
}
