using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadAsync(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
