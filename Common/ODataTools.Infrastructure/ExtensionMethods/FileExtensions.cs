using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.Infrastructure.ExtensionMethods
{
    public static class FileExtensions
    {
        public static Task<string> ReadAllTextAsync(string path)
        {
            return ReadAllTextAsync(path, Encoding.UTF8);
        }

        public static async Task<string> ReadAllTextAsync(string path, Encoding encoding)
        {
            String text = string.Empty;

            using (var reader = new StreamReader(path, encoding))
            {
                text = await reader.ReadToEndAsync();                
            }

            return text;
        }
    }
}