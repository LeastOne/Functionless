namespace System
{
    public static class UriExtensions
    {
        public static Uri AppendPath(this Uri baseUri, string path)
        {
            return new Uri(baseUri, path);
        }
    }
}
