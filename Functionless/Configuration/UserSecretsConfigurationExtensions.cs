using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.Configuration
{
    internal static class UserSecretsConfigurationExtensions
    {
        internal static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, IEnumerable<Assembly> assemblies, bool optional, bool reloadOnChange)
        {
            foreach (var assembly in assemblies)
            {
                configuration = configuration.AddUserSecrets(assembly, optional, reloadOnChange);
            }

            return configuration;
        }
    }
}
