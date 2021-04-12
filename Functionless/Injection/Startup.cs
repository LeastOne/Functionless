using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Autofac;
using Autofac.Extensions.DependencyInjection.AzureFunctions;

[assembly: InternalsVisibleTo("Functionless.Tests")]

namespace Functionless.Injection
{
    public abstract class Startup : FunctionsStartup
    {
        private static IDictionary<Startup, bool> Startups = new Dictionary<Startup, bool>();

        public static IEnumerable<Assembly> HostAssemblies => from p in Startups where p.Value select p.Key.GetType().Assembly;

        public Startup()
        {
            Startups[this] = false;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            Startups[this] = true;

            if (Startups.Any(p => !p.Value)) return;

            var serviceDescriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));

            var originalConfig = (
                serviceDescriptor.ImplementationInstance ??
                serviceDescriptor.ImplementationFactory(default)
            ) as IConfiguration;

            var newConfig = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddConfiguration(originalConfig)
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets(HostAssemblies, true, true)
                .Build();

            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), newConfig));

            builder.UseAutofacServiceProviderFactory(
                b => Startups.Keys.ToList().ForEach(a => a.Load(b))
            );
        }

        public virtual void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<Module>();
        }
    }
}
