namespace System.Reflection
{
    internal static class MethodInfoExtensions
    {
        internal static MethodInfo TryMakeGenericMethod(this MethodInfo method, Type[] genericParameters)
        {
            return method.IsGenericMethod
                ? method.MakeGenericMethod(genericParameters)
                : method;
        }
    }
}
