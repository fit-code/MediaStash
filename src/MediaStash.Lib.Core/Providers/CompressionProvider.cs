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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using System.IO;
using System.IO.Compression;

namespace Fitcode.MediaStash.Lib.Providers
{
    public class CompressionProvider : ICompressionProvider
    {
        public ICompressionConfiguration Config { get; private set; }

        public CompressionProvider(ICompressionConfiguration config)
        {
            Config = config;
        }

        public CompressedPack Pack(string name, IEnumerable<IMedia> mediaCollection)
        {
            return PackAsync(name, mediaCollection).Result;
        }

        public CompressedPack Pack(string name, MemoryMediaContainer memoryMediaContainer)
        {
            return PackAsync(name, memoryMediaContainer).Result;
        }

        public CompressedPack Pack(string name, MediaContainer mediaContainer)
        {
            return PackAsync(name, mediaContainer).Result;
        }

        public async Task<CompressedPack> PackAsync(string name, IEnumerable<IMedia> mediaCollection)
        {
            var memoryStream = new MemoryStream();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in mediaCollection)
                {
                    var entryFile = archive.CreateEntry(file.Name, CompressionLevel.Optimal);

                    using (var entryStream = entryFile.Open())
                    {
                        await entryStream.WriteAsync(file.Data, 0, file.Data.Length);
                    }
                }
            }

            return new CompressedPack(name, memoryStream);
        }

        public async Task<CompressedPack> PackAsync(string name, MemoryMediaContainer memoryMediaContainer)
        {
            return await PackAsync(name, memoryMediaContainer.Media);
        }

        public async Task<CompressedPack> PackAsync(string name, MediaContainer mediaContainer)
        {
            return await PackAsync(name, mediaContainer.Media);
        }

        public MediaContainer Unpack(MemoryStream stream)
        {
            return UnpackAsync(stream).Result;
        }

        public MediaContainer Unpack(byte[] buffer)
        {
            return UnpackAsync(new MemoryStream(buffer)).Result;
        }

        public async Task<MediaContainer> UnpackAsync(MemoryStream stream)
        {
            var container = new MediaContainer();
            var media = new List<GenericMedia>();

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    if (Config.SupportedExtensions.Contains(Path.GetExtension(entry.FullName), StringComparer.OrdinalIgnoreCase))
                    {
                        using (var tempStream = entry.Open())
                        {
                            using (var destinationStream = new MemoryStream())
                            {
                                await tempStream.CopyToAsync(destinationStream);
                                
                                media.Add(new GenericMedia(entry.FullName, destinationStream.ToArray()));
                            }
                        }
                    }
                }
            }

            container.Media = media;

            // Cleanup
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            return container;
        }

        public async Task<MediaContainer> UnpackAsync(byte[] buffer)
        {
            return await UnpackAsync(new MemoryStream(buffer));
        }
    }
}
