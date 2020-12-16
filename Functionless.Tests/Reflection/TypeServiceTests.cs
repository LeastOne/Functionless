using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Functionless.Reflection;

namespace Functionless.Tests.Reflection
{
    [TestClass]
    public class TypeServiceTests
    {
        [DataTestMethod]
        [DataRow("GenericClass`1[Object].<GenericMethod>()", null, "211c2f1c-fb23-4445-832e-a69c13d91193")]
        [DataRow("GenericClass`1[String].<GenericMethod>(String)", new object[] { "" }, "70a272fd-e9f6-4e10-9d81-116ddc491e74")]
        [DataRow("GenericClass`1[Object].<GenericMethod>`1[String](String)", new object[] { "" }, "17dc60ce-31b3-4e3a-887a-255e02474091")]
        [DataRow("GenericClass`1[Object].<GenericMethod>`2[Object,String](Object,String)", new object[] { null, "" }, "6af87587-8209-460e-9f73-0159beb15d0c")]
        [DataRow("GenericClass`1[Object].<GenericMethod>`2[Object,String](Object,String,Boolean)", new object[] { null, "", false }, "8293e30e-30de-4553-a473-7e3e0ecaaa9c")]
        [DataRow("GenericClass`1[Object].<GenericMethod>`2[Object,String](Object,String,Boolean,Int32)", new object[] { null, "", false, 0 }, "0683c395-d62f-43ba-9d9e-600358304182")]
        [DataRow("GenericClass`1[System.Collections.Generic.Dictionary`2[Object,Object]].<GenericMethod>`2[System.Collections.Generic.Dictionary`2[Object, Object],System.Collections.Generic.Dictionary`2[String,String]](System.Collections.Generic.Dictionary`2[Object,Object],System.Collections.Generic.Dictionary`2[String,String],Boolean,Int32)", new object[] { null, null, false, 0 }, "0683c395-d62f-43ba-9d9e-600358304182")]
        public void GetMethodTest(string spec, object[] arguments, string expected)
        {
            var typeService = new TypeService();
            var method = typeService.GetMethod(spec);
            var instance = Activator.CreateInstance(method.DeclaringType);
            method.Invoke(instance, arguments).Should().Be(expected);
            var spec2 = typeService.GetMethodSpecification(method);
            var method2 = typeService.GetMethod(spec2);
            method2.Should().BeSameAs(method);
        }
    }

    public class GenericClass<T>
    {
        public string GenericMethod()
        {
            // NOTE: The use of a lambda here forces an anonymous function to be compiled
            // into the class which is expected to be ignored by the TypeService.
            var temp = new object[0].Select(s => s.ToString() == "");

            return "211c2f1c-fb23-4445-832e-a69c13d91193";
        }

        public string GenericMethod(T entity)
        {
            return "70a272fd-e9f6-4e10-9d81-116ddc491e74";
        }

        public string GenericMethod<T1>(T1 entity)
        {
            return "17dc60ce-31b3-4e3a-887a-255e02474091";
        }

        public string GenericMethod<T1, T2>(T1 entity, T2 entity2)
        {
            return "6af87587-8209-460e-9f73-0159beb15d0c";
        }

        public string GenericMethod<T1, T2>(T1 entity, T2 entity2, bool flag)
        {
            return "8293e30e-30de-4553-a473-7e3e0ecaaa9c";
        }

        public string GenericMethod<T1, T2>(T1 entity, T2 entity2, bool flag, int number = 0)
        {
            return "0683c395-d62f-43ba-9d9e-600358304182";
        }
    }
}
