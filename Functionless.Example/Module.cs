using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;

using Autofac;

namespace Functionless.Example
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Assembly
                .GetExecutingAssembly().GetTypes()
                .Where(p => !p.IsInterface && p.IsAssignableTo<IConfig>())
                .ToList().ForEach(
                    a => builder.Register(
                        c => c.Resolve<IConfiguration>().GetSection(a.Name)?.Get(a) ?? Activator.CreateInstance(a)
                    ).As(a).SingleInstance()
                );
        }
    }
}
