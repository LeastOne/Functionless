using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

namespace Functionless.Tests.Default
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void JoinTest()
        {
            Enumerable.Range(1, 3).Select(s => s.ToString()).Join(",").Should().Be("1,2,3");
        }
    }
}
