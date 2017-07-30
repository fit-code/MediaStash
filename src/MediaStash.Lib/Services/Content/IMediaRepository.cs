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

using MediaStash.Lib.Helpers;
using MediaStash.Lib.Services.Processing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaStash.Lib.Services.Content
{
    /// <summary>
    /// Media repository contract for all concrete service (Azure, Amazon, Dropbox, etc)
    /// </summary>
    public interface IMediaRepository
    {
        /// <summary>
        /// Configuration contract
        /// </summary>
        IRepositoryConfiguration Config { get; }

        /// <summary>
        /// Providers perform transformations to the content before uploading and directly to the downloaded stream.
        /// </summary>
        IEnumerable<IProvider> Providers { get; }

        /// <summary>
        /// Applies all provider transformations to an IMedia implementation.
        /// </summary>
        /// <param name="media">IMedia concrete implementation.</param>
        /// <returns>Task (IMedia.Data is modified directly)</returns>
        Task RunProviderProcess(IMedia media);

        /// <summary>
        /// Reverses transformations applied by provider collection.
        /// </summary>
        /// <param name="media">IMedia concreate type.</param>
        /// <returns>Task (IMedia.Data is modified directly)</returns>
        Task ReverseProvider(IMedia media);

        /// <summary>
        /// Store a collection of media elements grouped by container.
        /// </summary>
        /// <param name="mediaContainer">Media Container</param>
        /// <returns>Task.Completed unless exception is thrown.</returns>
        Task StashContainerAsync(IMediaContainer mediaContainer);

        /// <summary>
        /// Store a collection of media elements grouped by container.
        /// </summary>
        /// <param name="mediaContainer">Media Container</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <returns>Task.Completed unless exception is thrown.</returns>
        Task StashContainerAsync(IMediaContainer mediaContainer, string storageContainer);

        /// <summary>
        /// Store a collection of media passed as IEnumerable.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="mediaCollection">IEnumerable of IMedia contract.</param>
        /// <returns>Task.Completed unless exception is thrown.</returns>
        Task StashMediaAsync(string path, IEnumerable<IMedia> mediaCollection);

        /// <summary>
        /// Store a collection of media passed as IEnumerable.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <param name="mediaCollection">IEnumerable of IMedia contract.</param>
        /// <returns>Task.Completed unless exception is thrown.</returns> 
        Task StashMediaAsync(string path, string storageContainer, IEnumerable<IMedia> mediaCollection);

        /// <summary>
        /// Create and return media container found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <returns>Concrete IMediaContainer implementation <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IMediaContainer> GetMediaContainerAsync(string path, bool loadResourcePathOnly = false);

        /// <summary>
        /// Create and return media container found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <returns>Concrete IMediaContainer implementation <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IMediaContainer> GetMediaContainerAsync(string path, string storageContainer, bool loadResourcePathOnly);

        /// <summary>
        /// Return IEnumerable of IMedia found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="loadResourcePathOnly">Flag used for public containers so we don't download file, just grab the path.</param>
        /// <returns>Collection of IMedia <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IEnumerable<IMedia>> GetMediaAsync(string path, bool loadResourcePathOnly = false);

        /// <summary>
        /// Return IEnumerable of IMedia found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="loadResourcePathOnly">Flag used for public containers so we don't download file, just grab the path.</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <returns>Collection of IMedia <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IEnumerable<IMedia>> GetMediaAsync(string path, string storageContainer, bool loadResourcePathOnly);

        /// <summary>
        /// Persist directory to cloud storage.
        /// </summary>
        /// <param name="path">Directory path subject to validation.</param>
        /// <param name="rootStorageContainer">Cloud container</param>
        /// <param name="includeSubDirectory">Flag telling the repo to stash sub directory content.</param>
        /// <returns>IDirectoryResult implementation.</returns>
        Task<IDirectoryResult> StashDirectoryAsync(string path, bool includeSubDirectory = false);

        Task<IDirectoryResult> StashDirectoryAsync(string path, string rootContainer, bool includeSubDirectory = false);

        // Events
        event Notify OnDirectoryStash;
    }
}
