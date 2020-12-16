using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Functionless.IO
{
    public class MethodPathHelper : PathHelper
    {
        private static Regex AssemblyEntryMethodRegex = new Regex($@"^(System)|({typeof(MethodPathHelper).Assembly.GetName().Name})", RegexOptions.Compiled);

        public override string PartsToPath(IEnumerable<object> parts)
        {
            return this.PartsToPath(this.GetAssemblyEntryMethod(), parts);
        }

        public string PartsToPath(MethodBase method, IEnumerable<object> parts)
        {
            return base.PartsToPath(
                this.GetMethodParts(method).Concat(parts)
            );
        }

        protected virtual MethodBase GetAssemblyEntryMethod()
        {
            var callstack = (
                from frame in EnhancedStackTrace.Current()
                let method = frame.MethodInfo
                where method?.DeclaringType?.FullName?.StartsWith("System") != true
                select $"{method?.DeclaringType?.FullName}.{method?.Name}"
            ).ToArray();

            var methodBase = (
                from frame in EnhancedStackTrace.Current()
                let method = frame.MethodInfo
                where method.DeclaringType.GetCustomAttribute<CompilerGeneratedAttribute>() == null
                && !AssemblyEntryMethodRegex.IsMatch(method.DeclaringType.FullName)
                select method.MethodBase
            ).First();

            return methodBase;
        }

        protected virtual IList<string> GetMethodParts(MethodBase method)
        {
            var assemblyName = method.DeclaringType.Assembly.GetName().Name;
            var targetTypeName = method.DeclaringType.FullName.Replace($"{assemblyName}.", string.Empty);
            return targetTypeName.Split(".").Union(new[] { method.Name }).ToList();
        }
    }
}
