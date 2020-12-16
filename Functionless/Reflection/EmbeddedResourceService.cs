using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Functionless.Reflection
{
    public class EmbeddedResourceService
    {
        public virtual Stream GetResourceStream(string pattern)
        {
            var executingAssemblyName = typeof(EmbeddedResourceService).Assembly.GetName();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(
                p => !p.IsDynamic && p.GetReferencedAssemblies().Any(q => q.FullName == executingAssemblyName.FullName)
            );

            var regex = new Regex(pattern);

            return (
                from assembly in assemblies
                from name in assembly.GetManifestResourceNames()
                where regex.IsMatch(name)
                select assembly.GetManifestResourceStream(name)
            ).Single();
        }
    }
}
