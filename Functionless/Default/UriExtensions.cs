namespace System
{
    internal static class UriExtensions
    {
        internal static Uri AppendPath(this Uri baseUri, string path)
        {
            return new Uri(baseUri, path);
        }
    }
}
