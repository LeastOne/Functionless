using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    internal static class AssemblyExtensions
    {
        internal static IEnumerable<Assembly> RelatedTo(this IEnumerable<Assembly> assemblies, Assembly assembly)
        {
            return assemblies.Where(
                p => !p.IsDynamic && (
                    p.FullName == assembly.FullName ||
                    p.GetReferencedAssemblies().Any(q => q.FullName == assembly.FullName)
                )
            );
        }

        internal static Type[] GetTypesOrDefault(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return new Type[0];
            }
        }
    }
}
