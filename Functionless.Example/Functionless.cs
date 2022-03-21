using Autofac;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Functionless.Example.Functionless))]

namespace Functionless.Example
{
    public partial class Functionless : Injection.Startup
    {
        public override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<Module>();

            base.Load(builder);
        }
    }
}