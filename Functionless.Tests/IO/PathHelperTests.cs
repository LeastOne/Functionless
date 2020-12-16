using Microsoft.VisualStudio.TestTools.UnitTesting;

using Functionless.IO;

namespace Functionless.Tests.IO
{
    [TestClass]
    public class PathHelperTests
    {
        [TestMethod]
        public void PartsToPathTest()
        {
            var pathHelper = new PathHelper();

            var actual = pathHelper.PartsToPath(
                pathHelper.PathToParts(@"alpha\branvo\charlie\..\..\delta\echo\..\blob.json")
            );

            Assert.AreEqual(@"alpha/delta/blob.json", actual);
        }
    }
}
