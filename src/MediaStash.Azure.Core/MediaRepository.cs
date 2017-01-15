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

        public MediaRepository(IRepositoryConfiguration config, IEnumerable<IProvider> providers) : this(config)
        {
            this.Providers = providers;
        }

        public IRepositoryConfiguration Config { get; private set; }
        public IEnumerable<IProvider> Providers { get; private set; }

        public async Task RunProviderProcess(IMedia media)
        {
            if (media.Metadata == null)
                media.Metadata = new Dictionary<string, string>();

            if (Providers != null && Providers.Count() > 0)
            {
                foreach (var provider in Providers)
                {
                    media.Data = await provider.ProcessAsync(media.Data);
                    media.Metadata.Add(provider.GetType().Name, provider.GetType().FullName);
                }
            }
        }

        public async Task ReverseProvider(IMedia media)
        {
            if (Providers != null && Providers.Count() > 0)
            {
                foreach (var provider in Providers.Reverse()) // We wan't to reverse for the correct processing order
                {
                    if (media.Metadata.ContainsKey(provider.GetType().Name))
                        media.Data = await provider.ReverseAsync(media.Data);
                }
            }
        }

        public async Task StashContainerAsync(IMediaContainer mediaContainer)
        {
            await StashContainerAsync(mediaContainer, Config.RootContainer);
        }

        public async Task StashContainerAsync(IMediaContainer mediaContainer, string storageContainer)
        {
            CloudBlobContainer rootContainer = _blobClient.GetContainerReference(storageContainer);

            if (await rootContainer.ExistsAsync())
            {
                foreach (var file in mediaContainer.Media)
                {
                    CloudBlockBlob blob = rootContainer.GetBlockBlobReference($@"{mediaContainer.Path}\{file.Name}");

                    await RunProviderProcess(file);

                    // Append metadata
                    if (blob.Metadata != null && file.Metadata.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> entry in file.Metadata)
                            blob.Metadata.Add(entry);
                    }

                    await blob.UploadFromByteArrayAsync(file.Data, 0, file.Data.Length);

                    file.Uri = blob.Uri.ToString();
                }
            }
            else
            {
                throw new InvalidOperationException($"Container <{Config.RootContainer}> does not exist.");
            }
        }

        public async Task StashMediaAsync(string path, IEnumerable<IMedia> mediaCollection)
        {
            await StashMediaAsync(path, Config.RootContainer, mediaCollection);
        }

        public async Task StashMediaAsync(string path, string storageContainer, IEnumerable<IMedia> mediaCollection)
        {
            await StashContainerAsync(new MediaContainer
            {
                Path = path,
                Media = mediaCollection.Select(s => new GenericMedia(s.Name, s.Data)).AsEnumerable()
            }, storageContainer);
        }

        public async Task<IMediaContainer> GetMediaContainerAsync(string path, bool loadResourcePathOnly = false)
        {
            return await GetMediaContainerAsync(path, Config.RootContainer, loadResourcePathOnly);
        }

        public async Task<IMediaContainer> GetMediaContainerAsync(string path, string storageContainer, bool loadResourcePathOnly)
        {
            CloudBlobContainer rootContainer = _blobClient.GetContainerReference(storageContainer);

            if (await rootContainer.ExistsAsync())
            {
                string prefix = $@"{path.Replace(@"\", "/")}/";

                BlobResultSegment segment = await rootContainer.ListBlobsSegmentedAsync(prefix, new BlobContinuationToken());
                var container = new MediaContainer
                {
                    Path = path
                };
                var media = new List<GenericMedia>();

                foreach (IListBlobItem item in segment.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blockBlob = item as CloudBlockBlob;

                        if (loadResourcePathOnly)
                        {
                            media.Add(new GenericMedia(Path.GetFileName(blockBlob.Name), data: null)
                            {
                                Uri = blockBlob.Uri.ToString()
                            });
                        }
                        else
                        {
                            using (var file = new MemoryStream())
                            {
                                await blockBlob.DownloadToStreamAsync(file);

                                var mediaFile = new GenericMedia(Path.GetFileName(blockBlob.Name), file.ToArray())
                                {
                                    Uri = blockBlob.Uri.ToString(),
                                    Metadata = new Dictionary<string, string>()
                                };

                                if (blockBlob.Metadata!=null)
                                {
                                    foreach (KeyValuePair<string, string> entry in blockBlob.Metadata)
                                        mediaFile.Metadata.Add(entry.Key, entry.Value);
                                }

                                await ReverseProvider(mediaFile);

                                media.Add(mediaFile);
                            }
                        }
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

        public Task<IEnumerable<IMedia>> GetMediaAsync(string path, bool loadResourcePathOnly = false)
        {
            return GetMediaAsync(path, Config.RootContainer, loadResourcePathOnly);
        }

        public async Task<IEnumerable<IMedia>> GetMediaAsync(string path, string storageContainer, bool loadResourcePathOnly)
        {
            return (await GetMediaContainerAsync(path, storageContainer, loadResourcePathOnly))?.Media;
        }
    }
}
