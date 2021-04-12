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
        private static bool loaded = false;

        protected override void Load(ContainerBuilder builder)
        {
            if (loaded) return; loaded = true;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().RelatedTo(
                Assembly.GetExecutingAssembly()
            ).ToList();

            var interceptorTypes =
                assemblies
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.IsAssignableTo<IInterceptor>())
                    .OrderBy(t => t.GetCustomAttribute<PrecedenceAttribute>()?.Value ?? double.MaxValue)
                    .ToArray();

            builder.RegisterTypes(interceptorTypes).AsSelf().SingleInstance();

            assemblies.ForEach(
                assembly => builder
                    .RegisterAssemblyTypes(assembly).PublicOnly()
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
