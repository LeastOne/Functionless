using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Functionless.Reflection;

namespace Functionless.Tests.Reflection
{
    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        public void GetCollectionNameTest()
        {
            typeof(IDictionary<object, string>).GetCollectionName().Should().Be("ObjectString");
        }
    }
}
