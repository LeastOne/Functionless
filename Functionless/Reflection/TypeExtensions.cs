using System.Linq;

using Functionless.Durability;

namespace System.Reflection
{
    internal static class TypeExtensions
    {
        internal static Type TryMakeGenericType(this Type type, Type[] genericParameters)
        {
            return type.IsGenericType
                ? type.MakeGenericType(genericParameters)
                : type;
        }
    }
}
