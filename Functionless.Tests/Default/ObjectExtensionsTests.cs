using System;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Functionless.Tests.Default
{
    [TestClass]
    public class ObjectExtensionsTests
    {
        private string Byte = "76";

        private string Short = short.MaxValue.ToString();

        private string Int = int.MaxValue.ToString();

        private string Long = long.MaxValue.ToString();

        private string Float = float.MaxValue.ToString();

        private string Double = double.MaxValue.ToString();

        private string Char = 'a'.ToString();

        private string String = "a";

        private string EnumString = TestEnum.A.ToString();

        private int EnumInt = (int)TestEnum.B;

        private string Datetime = DateTime.UtcNow.ToString();

        private string Timespan = "03:02:01";

        [TestMethod]
        public void ChangeTypeTests()
        {
            Byte.ChangeType<byte>().Should().Be(byte.Parse(Byte));
            Short.ChangeType<short>().Should().Be(short.Parse(Short));
            Int.ChangeType<int>().Should().Be(int.Parse(Int));
            Int.ChangeType<int?>().Should().Be(int.Parse(Int) as int?);
            string.Empty.ChangeType<int?>().Should().BeNull();
            Long.ChangeType<long>().Should().Be(long.Parse(Long));
            Float.ChangeType<float>().Should().Be(float.Parse(Float));
            Double.ChangeType<double>().Should().Be(double.Parse(Double));
            Char.ChangeType<char>().Should().Be(char.Parse(Char));
            String.ChangeType<string>().Should().Be(String);
            EnumString.ChangeType<TestEnum>().Should().Be(Enum.Parse<TestEnum>(EnumString));
            EnumInt.ChangeType<TestEnum>().Should().Be((TestEnum)EnumInt);
            Datetime.ChangeType<DateTime>().Should().Be(DateTime.Parse(Datetime));
            Timespan.ChangeType<TimeSpan>().Should().Be(TimeSpan.Parse(Timespan));

            Byte.ChangeType(typeof(short)).Should().Be(byte.Parse(Byte));
            Short.ChangeType(typeof(short)).Should().Be(short.Parse(Short));
            Int.ChangeType(typeof(int)).Should().Be(int.Parse(Int));
            Long.ChangeType(typeof(long)).Should().Be(long.Parse(Long));
            Float.ChangeType(typeof(float)).Should().Be(float.Parse(Float));
            Double.ChangeType(typeof(double)).Should().Be(double.Parse(Double));
            Char.ChangeType(typeof(char)).Should().Be(char.Parse(Char));
            String.ChangeType(typeof(string)).Should().Be(String);
            EnumString.ChangeType(typeof(TestEnum)).Should().Be(Enum.Parse<TestEnum>(EnumString));
            EnumInt.ChangeType(typeof(TestEnum)).Should().Be((TestEnum)EnumInt);
            Datetime.ChangeType(typeof(DateTime)).Should().Be(DateTime.Parse(Datetime));
            Timespan.ChangeType(typeof(TimeSpan)).Should().Be(TimeSpan.Parse(Timespan));
        }
    }

    public enum TestEnum { A, B, C }
}
