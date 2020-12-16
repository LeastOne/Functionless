using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Functionless.Reflection;

namespace Functionless.Tests.Reflection
{
    [TestClass]
    public class EmbeddedResourceSerivceTests
    {
        private readonly string expected = "3a8dd475-95bf-4128-b893-b256556adb62";

        private readonly EmbeddedResourceService embeddedResourceService = new EmbeddedResourceService();

        [DataTestMethod]
        [DataRow("Test.txt", false)]
        [DataRow("Data.Test.txt", false)]
        [DataRow("Invalid", true)]
        public void GetResourceTest(string pattern, bool shouldThrow)
        {
            Action assertion = () => {
                using (var stream = embeddedResourceService.GetResourceStream(pattern))
                using (var reader = new StreamReader(stream))
                {
                    reader.ReadToEnd().Should().Be(expected);
                }
            };
            
            if (shouldThrow)
            {
                assertion.Should().Throw<Exception>();
            }
            else
            {
                assertion.Should().NotThrow();
            }
        }
    }
}
