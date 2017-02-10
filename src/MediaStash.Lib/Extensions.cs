#region License
// Copyright (c) 2017 Fitcode.io (info@fitcode.io)
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using Fitcode.MediaStash.Lib.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Fitcode.MediaStash.Lib
{
    public static class Extensions
    {
        ///// <summary>
        ///// Grab the byte array from IFile concrete type.
        ///// </summary>
        ///// <param name="file">IFile</param>
        ///// <returns>byte[stream.Length]</returns>
        //public static async Task<byte[]> ToByteArray(this IFile file)
        //{
        //    if (file == null) return null;

        //    using (var stream = await file.OpenAsync(FileAccess.Read))
        //    {
        //        var buffer = new byte[stream.Length];

        //        stream.Read(buffer, 0, buffer.Length);

        //        return buffer;
        //    }
        //}

        /// <summary>
        /// Grab the byte array for any stream.
        /// </summary>
        /// <param name="stream">Stream child.</param>
        /// <param name="autoDispose">Cleanup orignal Stream once we have the data.</param>
        /// <returns>byte[stream.Length]</returns>
        public static byte[] ToByteArray(this Stream stream, bool autoDispose = false)
        {
            if (stream == null) return null;

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            if (autoDispose)
            {
                stream.Dispose();
                stream = null;
            }

            return buffer;
        }

        /// <summary>
        /// Simple buffer to MemoryStream conversion with safety check.
        /// </summary>
        /// <param name="buffer">byte[]</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream ToMemory(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return null;

            return new MemoryStream(buffer);
        }

        /// <summary>
        /// Wrap hash computing into extension.
        /// </summary>
        /// <param name="buffer">byte[]</param>
        /// <returns>HEX Hash</returns>
        public static string ComputeHash(this byte[] buffer)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(buffer);
                return string.Concat(hash.Select(x => x.ToString("X2")));
            }
        }

        public static byte[] ToByArray(this FileInfo fileInfo)
        {
            byte[] result = null;

            using (var stream = fileInfo.OpenRead())
                result = stream.ToByteArray(false);

            return result;
        }

        public static IList<DirectoryOperation> ToOperations(this DirectoryInfo directoryInfo, string prefix = null)
        {
            var operations = new List<DirectoryOperation>();
            var files = directoryInfo.GetFiles();

            Parallel.ForEach(files, (f) =>
            {
                operations.Add(new DirectoryOperation
                {
                    Prefix = $"{prefix ?? directoryInfo.Name}",
                    FileName = f.Name,
                    FileData = f.ToByArray()
                });
            });

            return operations;
        }
    }
}
