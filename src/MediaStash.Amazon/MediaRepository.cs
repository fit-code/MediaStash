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
using Fitcode.MediaStash.Lib.Models;
using Fitcode.MediaStash.Lib.Abstractions;
using Amazon.S3;
using Amazon.S3.Model;
using Fitcode.MediaStash.Lib.Helpers;
using Fitcode.MediaStash.Lib;

namespace Fitcode.MediaStash.AmazonStorage
{
    public class MediaRepository : IAmazonMediaRepository, IDisposable
    {
        private AmazonS3Client _s3Client = null;

        public event Notify OnDirectoryStash = null;

        public MediaRepository(IRepositoryConfiguration config)
        {
            this.Config = config;

            if (string.IsNullOrEmpty(config.Account.ServiceUrl))
            {
                // TODO: Expose region
                _s3Client = new AmazonS3Client(config.Account.Key, config.Account.Secret, Amazon.RegionEndpoint.USEast1);
            }
            else
            {
                _s3Client = new AmazonS3Client(config.Account.Key, config.Account.Secret,
                    new AmazonS3Config
                    {
                        ServiceURL = config.Account.ServiceUrl
                    });
            }
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

        public async Task<IEnumerable<S3Object>> ListObjectRequest(string storageContainer, string prefix)
        {
            var request = new ListObjectsRequest();
            request.BucketName = storageContainer;
            request.Prefix = prefix;

            ListObjectsResponse response = await _s3Client.ListObjectsAsync(request);

            return response.S3Objects;
        }

        public async Task StashContainerAsync(IMediaContainer mediaContainer)
        {
            await StashContainerAsync(mediaContainer, Config.RootContainer);
        }

        public async Task StashContainerAsync(IMediaContainer mediaContainer, string storageContainer)
        {
            foreach (var file in mediaContainer.Media)
            {
                using (var stream = new MemoryStream(file.Data))
                {
                    PutObjectRequest request = new PutObjectRequest();
                    request.AutoCloseStream = true;
                    request.BucketName = storageContainer;
                    request.CannedACL = S3CannedACL.PublicRead;
                    request.Key = $@"{mediaContainer.Path}/{file.Name}";
                    request.InputStream = stream;
                    
                    await RunProviderProcess(file);

                    // Append metadata
                    if (file.Metadata.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> entry in file.Metadata)
                            request.Metadata.Add(entry.Key, entry.Value);
                    }

                    PutObjectResponse response = await _s3Client.PutObjectAsync(request);
              
                    file.Uri = $@"https://s3.amazonaws.com/{storageContainer}/{mediaContainer.Path}/{file.Name}";
                }
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
            var prefix = $@"{path.Replace(@"\", "/")}/";
            var files = await ListObjectRequest(storageContainer, prefix);
            var container = new MediaContainer
            {
                Path = $"https://s3.amazonaws.com/{storageContainer}/{path}"
            };
            var media = new List<GenericMedia>();

            // Validate empty/null
            if (files == null && files.Count() == 0)
                return container;

            // Only resources path
            if (loadResourcePathOnly)
            {
                foreach (var item in files)
                {
                    // Fix for amazon delete/persistance of files.
                    if (!item.Key.Replace("/", string.Empty).EndsWith(path))
                    {
                        media.Add(new GenericMedia(Path.GetFileName(item.Key), data: null)
                        {
                            Uri = $"https://s3.amazonaws.com/{storageContainer}/{item.Key}"
                        });
                    }
                }

                container.Media = media;

                return container;
            }

            foreach (var item in files)
            {
                var amazonFile = await _s3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = storageContainer,
                    Key = item.Key
                });

                using (var file = new MemoryStream())
                {
                    await amazonFile.ResponseStream.CopyToAsync(file);
             
                    var mediaFile = new GenericMedia(Path.GetFileName(item.Key), file.ToArray())
                    {
                        Uri = $@"https://s3.amazonaws.com/{storageContainer}/{item.Key}",
                        Metadata = new Dictionary<string, string>()
                    };

                    if (amazonFile.ResponseMetadata.Metadata != null)
                    {
                        foreach (var key in amazonFile.Metadata.Keys)
                            mediaFile.Metadata.Add(key, amazonFile.Metadata[key]);
                    }

                    await ReverseProvider(mediaFile);

                    if (mediaFile.Data != null && mediaFile.Data.Length > 0)
                        media.Add(mediaFile);
                }
            }

            container.Media = media;

            return container;
        }

        public Task<IEnumerable<IMedia>> GetMediaAsync(string path, bool loadResourcePathOnly = false)
        {
            return GetMediaAsync(path, Config.RootContainer, loadResourcePathOnly);
        }

        public async Task<IEnumerable<IMedia>> GetMediaAsync(string path, string storageContainer, bool loadResourcePathOnly)
        {
            return (await GetMediaContainerAsync(path, storageContainer, loadResourcePathOnly))?.Media;
        }

        public Task<IDirectoryResult> StashDirectoryAsync(string path, bool includeSubDirectory = false)
        {
            return StashDirectoryAsync(path, Config.RootContainer, includeSubDirectory);
        }

        public async Task<IDirectoryResult> StashDirectoryAsync(string path, string rootStorageContainer, bool includeSubDirectory = false)
        {
            if (Directory.Exists(path))
            {
                var rootDir = new DirectoryInfo(path);
                List<DirectoryOperation> operations = rootDir.ToOperations().ToList();

                if (includeSubDirectory)
                {
                    foreach (var subDir in rootDir.GetDirectories())
                    {
                        operations.AddRange(subDir.ToOperations($@"{rootDir.Name}\{subDir.Name}", true));
                    }
                }

                var notificationReport = new Notification
                {
                    TotalFiles = operations.Count,
                    TotalMegabytes = operations.Sum(s => s.FileData.Length).ConvertToMegabytes(),
                    ProcessedMegabytes = 0
                };

                if (OnDirectoryStash != null)
                    OnDirectoryStash(notificationReport);

                foreach (var operation in operations)
                {
                    using (var stream = new MemoryStream(operation.FileData))
                    {
                        PutObjectRequest request = new PutObjectRequest();
                        request.AutoCloseStream = true;
                        request.BucketName = rootStorageContainer;
                        request.CannedACL = S3CannedACL.PublicRead;
                        request.Key = operation.CloudPath.Replace(@"\", "/");
                        request.InputStream = stream;
                        
                        PutObjectResponse response = await _s3Client.PutObjectAsync(request);

                        notificationReport.ProcessedMegabytes += operation.FileData.Length.ConvertToMegabytes();

                        if (OnDirectoryStash != null)
                            OnDirectoryStash(notificationReport);
                    }
                }

                return new DirectoryResult(rootDir, rootStorageContainer, operations.Select(s =>
                    new Tuple<string, string>(s.OriginalPath, s.CloudPath)).ToList());
            }
            else
                throw new InvalidOperationException($"Invalid DirectoryPath: {path}");
        }

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_disposed) return;
            if (isDisposing)
            {
                _disposed = true;

                if (_s3Client != null)
                {
                    _s3Client.Dispose();
                    _s3Client = null;
                }
            }
        }
    }
}
