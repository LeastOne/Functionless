using System;
using System.Reflection;

namespace Functionless.Reflection
{
    public static class MethodInfoExtensions
    {
        public static MethodInfo TryMakeGenericMethod(this MethodInfo method, Type[] genericParameters)
        {
            return method.IsGenericMethod
                ? method.MakeGenericMethod(genericParameters)
                : method;
        }
    }
}
