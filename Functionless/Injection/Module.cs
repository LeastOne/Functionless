using System;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Extras.DynamicProxy;

using Castle.DynamicProxy;

using Functionless.Durability;

namespace Functionless.Injection
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().RelatedTo(
                Assembly.GetExecutingAssembly()
            ).ToList();

            builder.RegisterAssemblyModules(
                assemblies.Where(p => p != Assembly.GetExecutingAssembly()).ToArray()
            );

            var interceptorTypes =
                assemblies
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.IsAssignableTo<IInterceptor>())
                    .OrderBy(t => t.GetCustomAttribute<PrecedenceAttribute>()?.Value ?? int.MaxValue)
                    .ToArray();

            builder.RegisterTypes(interceptorTypes).AsSelf().SingleInstance();

            assemblies.ForEach(
                assembly => builder
                    .RegisterAssemblyTypes(assembly)
                    .Where(p => p.IsPublic && p.IsClass)
                    .AsSelf().AsImplementedInterfaces()
                    .PreserveExistingDefaults()
                    .EnableClassInterceptors()
                    .InterceptedBy(interceptorTypes)
            );

            builder.Register(
                c => new Lazy<DurableContext>(
                    () => DurableInterceptor.Context.Value
                )
            );
        }
    }
}
