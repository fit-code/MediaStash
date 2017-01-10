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
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Fitcode.MediaStash.Lib.Models;
using Fitcode.MediaStash.Lib.Abstractions;

namespace Fitcode.MediaStash.Azure
{
    public class MediaRepository : IMediaRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudBlobClient _blobClient;

        public MediaRepository(IRepositoryConfiguration config)
        {
            this.Config = config;

            _storageAccount = CloudStorageAccount.Parse(config.ConnectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        public IRepositoryConfiguration Config { get; private set; }

        public async Task StashContainer(IMediaContainer mediaContainer)
        {
            await StashContainer(mediaContainer, Config.RootContainer);
        }

        public async Task StashContainer(IMediaContainer mediaContainer, string storageContainer)
        {
            CloudBlobContainer rootContainer = _blobClient.GetContainerReference(storageContainer);

            if (await rootContainer.ExistsAsync())
            {
                foreach (var file in mediaContainer.Media)
                {
                    CloudBlockBlob blob = rootContainer.GetBlockBlobReference($@"{mediaContainer.Path}\{file.Name}");

                    await blob.UploadFromByteArrayAsync(file.Data, 0, file.Data.Length);
                }
            }
            else
            {
                throw new InvalidOperationException($"Container <{Config.RootContainer}> does not exist.");
            }
        }

        public async Task StashMedia(string path, IEnumerable<IMedia> mediaCollection)
        {
            await StashMedia(path, Config.RootContainer, mediaCollection);
        }

        public async Task StashMedia(string path, string storageContainer, IEnumerable<IMedia> mediaCollection)
        {
            await StashContainer(new MediaContainer
            {
                Path = path,
                Media = mediaCollection.Select(s => new GenericMedia(s.Name, s.Data)).AsEnumerable()
            }, storageContainer);
        }

        public async Task<IMediaContainer> GetMediaContainer(string path)
        {
            return await GetMediaContainer(path, Config.RootContainer);
        }

        public async Task<IMediaContainer> GetMediaContainer(string path, string storageContainer)
        {
            CloudBlobContainer rootContainer = _blobClient.GetContainerReference(storageContainer);

            if (await rootContainer.ExistsAsync())
            {
                string prefix = $@"{path.Replace(@"\", "/")}/";

                BlobResultSegment segment = await rootContainer.ListBlobsSegmentedAsync(prefix, new BlobContinuationToken());
                var container = new MemoryMediaContainer
                {
                    Path = path
                };
                var media = new List<MemoryStreamMedia>();

                foreach (IListBlobItem item in segment.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blockBlob = item as CloudBlockBlob;
                        var file = new MemoryStream();

                        await blockBlob.DownloadToStreamAsync(file);

                        media.Add(new MemoryStreamMedia(Path.GetFileName(blockBlob.Name), file));
                    }
                }

                container.Media = media;
                
                return container;
            }
            else
            {
                throw new InvalidOperationException($"Container <{Config.RootContainer}> does not exist.");
            }
        }

        public Task<IEnumerable<IMedia>> GetMedia(string path)
        {
            return GetMedia(path, Config.RootContainer);
        }

        public async Task<IEnumerable<IMedia>> GetMedia(string path, string storageContainer)
        {
            return (await GetMediaContainer(path, storageContainer))?.Media;
        }
    }
}
