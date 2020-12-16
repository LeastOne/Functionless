using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Assembly> RelatedTo(this IEnumerable<Assembly> assemblies, Assembly assembly)
        {
            var assemblyFullName = assembly.FullName;

            return assemblies.Where(
                p => !p.IsDynamic && (
                    p.FullName == assemblyFullName ||
                    p.GetReferencedAssemblies().Any(q => q.FullName == assemblyFullName)
                )
            );
        }

        public static Type[] GetTypesOrDefault(this Assembly assembly)
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
