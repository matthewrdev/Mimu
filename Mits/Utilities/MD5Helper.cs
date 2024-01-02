using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Mits.Utilities
{
    /// <summary>
    /// A helper class for generating MD5 hashes from strings, streams and file paths.
    /// </summary>
    public static class MD5Helper
    {
        /// <summary>
        /// Creates a new MD5 hash for the <paramref name="string"/>.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="string">Value.</param>
        public static string FromString(string @string)
        {
            if (string.IsNullOrEmpty(@string))
            {
                throw new ArgumentException($"'{nameof(@string)}' cannot be null or empty.", nameof(@string));
            }

            using (var md5 = MD5.Create())
            {
                var data = Encoding.UTF8.GetBytes(@string);
                return BitConverter.ToString(md5.ComputeHash(data));
            }
        }

        /// <summary>
        /// Creates a MD5 hash for the given <paramref name="stream"/>.
        /// </summary>
        /// <returns>The stream.</returns>
        /// <param name="stream">Data.</param>
        public static string FromStream(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(stream));
            }
        }

        /// <summary>
        /// Creates an MD5 has for the given <paramref name="filePath"/>.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="filePath">File path.</param>
        public static string FromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            using (var fs = File.OpenRead(filePath))
            {
                return FromStream(fs);
            }
        }
    }
}

