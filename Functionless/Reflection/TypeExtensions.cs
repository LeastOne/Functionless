using System;
using System.Linq;

namespace Functionless.Reflection
{
    public static class TypeExtensions
    {
        public static string GetCollectionName(this Type type)
        {
            return type.IsGenericType ?
                type.GenericTypeArguments.Select(p => p.Name).Join() :
                type.Name;
        }

        public static Type TryMakeGenericType(this Type type, Type[] genericParameters)
        {
            return type.IsGenericType
                ? type.MakeGenericType(genericParameters)
                : type;
        }
    }
}
