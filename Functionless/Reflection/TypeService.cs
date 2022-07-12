using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Functionless.Caching;
using Functionless.Injection;

namespace Functionless.Reflection
{
    public class TypeService
    {
        private static Regex TypeAndMethodSpecRegex = new Regex(@"^(?<Type>.*?)\.(?<Method>\<.*)$", RegexOptions.Compiled);

        private static Regex MethodSpecRegex = new Regex(@"^\<(?<Name>.*?)\>?(`\d+)?(\[(?<Generics>.*?)\])?\((?<Arguments>.*?)\)$", RegexOptions.Compiled);

        private static Regex TypeSpecRegex = new Regex(@"^(?<Name>.*?(`\d+)?)(\[(?<Generics>.*?)\])?$", RegexOptions.Compiled);

        private static Regex GenericSpecRegex = new Regex(@"[\w.]+(`\d+\[((?>[^\[\]]+|\((?<n>)|\)(?<-n>))+(?(n)(?!)))\])?", RegexOptions.Compiled);

        [MemoryCache("{0}")]
        public MethodInfo GetMethod(string spec)
        {
            var specMatch = TypeAndMethodSpecRegex.Match(spec.Trim());
            var typeSpec = specMatch.Groups["Type"].Value;
            var methodSpec = specMatch.Groups["Method"].Value;
            var type = this.GetType(typeSpec);
            return this.GetMethod(type, methodSpec);
        }

        [MemoryCache("{0}{1}")]
        public MethodInfo GetMethod(Type type, string spec)
        {
            var specMatch = MethodSpecRegex.Match(spec.Trim());
            var nameSpec = specMatch.Groups["Name"].Value;
            var genericSpec = specMatch.Groups["Generics"].Value;
            var argumentsSpec = specMatch.Groups["Arguments"].Value;
            var genericTypes = GenericSpecRegex.Matches(genericSpec).Cast<Match>().Select(s => this.GetType(s.Value)).ToArray();
            var argumentTypes = GenericSpecRegex.Matches(argumentsSpec).Cast<Match>().Select(s => this.GetType(s.Value)).ToArray();

            var matches = (
                from methodInfo in type.GetMethods()
                where methodInfo.Name == nameSpec
                && methodInfo.GetGenericArguments().Count() == genericTypes.Count()
                && methodInfo.GetParameters().Count() == argumentTypes.Count()
                && methodInfo.GetParameters()
                    .Select((p, i) => (p.ParameterType, Index: i))
                    .All(p => p.ParameterType.IsGenericParameter || p.ParameterType == argumentTypes[p.Index])
                select methodInfo
            ).ToList();

            if (matches.Count() != 1)
            {
                throw new AmbiguousMatchException(
                    $"The search for method {nameSpec} returned {matches.Count()} results when exactly 1 was expected: [{matches.Select(s => s.ToString()).Join(", ")}]"
                );
            }

            var method = matches.Single();

            return method.TryMakeGenericMethod(genericTypes);
        }

        [MemoryCache("{0}")]
        public Type GetType(string spec)
        {
            var specMatch = TypeSpecRegex.Match(spec.Trim());
            var nameSpec = specMatch.Groups["Name"].Value;
            var genericSpec = specMatch.Groups["Generics"].Value;
            var genericTypes = GenericSpecRegex.Matches(genericSpec).Cast<Match>().Select(s => this.GetType(s.Value)).ToArray();

            var matches = (
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from typeInfo in assembly.GetTypesOrDefault().DefaultIfEmpty().Cast<TypeInfo>()
                let exact = typeInfo?.FullName == nameSpec || typeInfo?.FullName == $"System.{nameSpec}" || Startup.HostAssemblies.Any(p => $"{p?.GetName()?.Name}.{nameSpec}" == typeInfo?.FullName)
                where typeInfo != null && !typeInfo.FullName.Contains("+") && typeInfo.FullName.Contains(nameSpec)
                && typeInfo.GenericTypeParameters.Count() == genericTypes.Count()
                orderby exact descending
                select (TypeInfo: typeInfo, Exact: exact)
            ).ToList();

            if (!matches.Any(p => p.Exact) && matches.Count() != 1)
            {
                var nonExactMatches = matches.Where(p => !p.Exact).ToList();

                throw new AmbiguousMatchException(
                    $"The search for type {nameSpec} returned {nonExactMatches.Count()} results when exactly 1 was expected: [{nonExactMatches.Select(s => s.TypeInfo.ToString()).Join(", ")}]"
                );
            }

            var type = matches.Select(s => s.TypeInfo).First();

            return type.TryMakeGenericType(genericTypes);
        }

        public string GetMethodSpecification(Type targetType, MethodInfo method)
        {
            var generics = string.Empty;

            if (method.IsGenericMethod)
            {
                var genericArguments = method.GetGenericArguments();
                generics = $"`{genericArguments.Count()}[{genericArguments.Select(this.GetTypeSpecification).ToList().Join(",")}]";
            }

            var parameters = method.GetParameters().Select(s => this.GetTypeSpecification(s.ParameterType)).ToList().Join(",");

            return $"{this.GetTypeSpecification(targetType)}.<{method.Name}>{generics}({parameters})";
        }

        public string GetTypeSpecification(Type type)
        {
            var generics = string.Empty;

            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments();
                generics = $"[{genericArguments.Select(this.GetTypeSpecification).ToList().Join(",")}]";
            }

            return $"{type.Namespace}.{type.Name}{generics}";
        }
    }
}
