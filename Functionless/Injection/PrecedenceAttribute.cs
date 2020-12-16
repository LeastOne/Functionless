using System;

namespace Functionless.Injection
{
    public class PrecedenceAttribute : Attribute
    {
        public PrecedenceAttribute(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }
}
