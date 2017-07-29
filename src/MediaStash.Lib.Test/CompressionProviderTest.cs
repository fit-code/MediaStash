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

using Fitcode.MediaStash.Lib;
using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using Fitcode.MediaStash.Lib.Providers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaStash.Lib.Test
{
    [TestFixture]
    public class CompressionProviderTest
    {
        private static ICompressionProvider _compressionProvider;
        private static ICompressionConfiguration _compressionConfiguration;

        private static string _filename = "anime.jpg";
        private static string _filePath = @"C:\Users\felip\Desktop\";

        [SetUp]
        public void Init()
        {
            _compressionConfiguration = new CompressionConfiguration();
            _compressionProvider = new CompressionProvider(_compressionConfiguration);
        }

        [TearDown]
        public void Cleanup()
        {
            //if (File.Exists($"{_filePath}packed{Path.GetFileName(_filename)}.zip"))
            //    File.Delete($"{_filePath}packed{Path.GetFileName(_filename)}.zip");
            //if (File.Exists($"{_filePath}unpacked-{_filename}"))
            //    File.Delete($"{_filePath}unpacked-{_filename}");
        }

        [Test]
        public static void TestContainerCompression()
        {
            var container = new MediaContainer
            {
                Media = new List<GenericMedia>
                {
                    new GenericMedia(_filename, new FileStream($"{_filePath}{_filename}", FileMode.Open).ToByteArray(true))
                }
            };

            var package = _compressionProvider.Pack($"packed{Path.GetFileName(_filename)}.zip", container);

            using (var writer = new FileStream($"{_filePath}{package.Name}", FileMode.OpenOrCreate))
            {
                var data = package.Package.ToArray();

                writer.Write(data, 0, data.Length);
            }

            Assert.IsTrue(File.Exists($"{_filePath}{package.Name}"));
        }

        [Test]
        public static void TestContainerUnpacking()
        {
            var mediaContainer = _compressionProvider.Unpack(new FileStream($"{_filePath}packed{Path.GetFileName(_filename)}.zip", FileMode.Open).ToByteArray(true));

            foreach (var media in mediaContainer.Media)
            {
                using (var writer = new FileStream($"{_filePath}unpacked-{media.Name}", FileMode.OpenOrCreate))
                {
                    var data = media.Data;

                    writer.Write(data, 0, data.Length);
                }

                Assert.IsTrue(File.Exists($"{_filePath}unpacked-{media.Name}"));
            }
        }
    }
}
