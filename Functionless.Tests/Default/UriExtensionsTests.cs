using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

namespace Functionless.Tests.Default
{
    [TestClass]
    public class UriExtensionsTests
    {
        [TestMethod]
        public void AppendPathTest()
        {
            var host = "http://localhost";
            var path = "/path";

            new Uri(host).AppendPath(path).ToString().Should().Be($"{host}{path}");
        }
    }
}
