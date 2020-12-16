using System;

namespace Functionless.Durability
{
    public class DurableInvocationException : Exception
    {
        public DurableInvocationException(string message) : base(message) { }
    }
}
