using System;
using System.Linq;
using System.Reflection;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Autofac;
using Autofac.Extensions.DependencyInjection.AzureFunctions;

namespace Functionless
{
    public abstract class Startup : FunctionsStartup
    {
        public static Assembly HostAssembly { get; private set; }

        public Startup(Assembly hostAssembly)
        {
            HostAssembly = hostAssembly;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceDescriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));

            var originalConfig = (
                serviceDescriptor.ImplementationInstance ??
                serviceDescriptor.ImplementationFactory(default)
            ) as IConfiguration;

            var newConfig = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddConfiguration(originalConfig)
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets(HostAssembly, true, true)
                .Build();

            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), newConfig));

            builder.UseAutofacServiceProviderFactory(
                b => b.RegisterModule<Injection.Module>()
            );
        }
    }
}
