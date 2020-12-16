using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

namespace Functionless.Tests.IO
{
    [TestClass]
    public class IOExtensionsTests
    {
        [TestMethod]
        public async Task ReadAsyncStreamTest()
        {
            var expected = Guid.NewGuid().ToString();
            var bytes = Encoding.Default.GetBytes(expected);

            using (var stream = new MemoryStream(bytes))
            {
                (await stream.ReadAsync()).Should().Be(expected);
            }
        }
    }
}
