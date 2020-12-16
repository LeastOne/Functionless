using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Functionless.IO
{
    public class TypePathHelper : MethodPathHelper
    {
        protected override IList<string> GetMethodParts(MethodBase method)
        {
            // NOTE: Its imparitive that the last item be removed here and not in `this.PartsToPath`
            // because its likely that `base.PartsToPath` will also be used in places.
            return base.GetMethodParts(method).SkipLast(1).ToList();
        }
    }
}
