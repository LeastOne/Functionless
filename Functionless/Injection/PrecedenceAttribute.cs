using System;

namespace Functionless.Injection
{
    public class PrecedenceAttribute : Attribute
    {
        public PrecedenceAttribute(double value)
        {
            this.Value = value;
        }

        public double Value { get; set; }
    }
}
