using System.Threading.Tasks;

namespace System.IO
{
    internal static class StreamExtensions
    {
        internal static async Task<string> ReadAsync(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
