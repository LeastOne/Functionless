using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Functionless.IO
{
    public class PathHelper
    {
        private static readonly Regex PathRegex = new Regex(@"[\\/]+", RegexOptions.Compiled);

        private static readonly Regex NameRegex = new Regex(@"[^\w-.]+", RegexOptions.Compiled);

        private static readonly Regex BackPathRegex = new Regex(@"(^|\w+[\\/])[.]{2}[\\/]", RegexOptions.Compiled);

        public virtual IList<string> PathToParts(object path)
        {
            if (path == null)
            {
                return Enumerable.Empty<string>().ToList();
            }

            return PathRegex.Split(path.ToString()).Select(
                s => NameRegex.Replace(s, "-")
            ).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
        }

        public virtual string PartToPath(object part)
        {
            return part is DateTime date ? date.ToString("yyyyMMddmmhhss") : NameRegex.Replace(part.ToString(), "-");
        }

        public virtual string PartsToPath(IEnumerable<object> parts)
        {
            var path = parts.Where(p => p != null).Select(PartToPath).Join("/");

            while (BackPathRegex.IsMatch(path))
            {
                path = BackPathRegex.Replace(path, string.Empty);
            }

            return path;
        }
    }
}
