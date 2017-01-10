﻿#region License
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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fitcode.MediaStash.Lib.Abstractions
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
        /// Store a collection of media elements grouped by container.
        /// </summary>
        /// <param name="mediaContainer">Media Container</param>
        /// <returns>Task.Completed unless exception is thrown.</returns>
        Task StashContainer(IMediaContainer mediaContainer);

        /// <summary>
        /// Store a collection of media elements grouped by container.
        /// </summary>
        /// <param name="mediaContainer">Media Container</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <returns>Task.Completed unless exception is thrown.</returns>
        Task StashContainer(IMediaContainer mediaContainer, string storageContainer);

        /// <summary>
        /// Store a collection of media passed as IEnumerable.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="mediaCollection">IEnumerable of IMedia contract.</param>
        /// <returns>Task.Completed unless exception is thrown.</returns>
        Task StashMedia(string path, IEnumerable<IMedia> mediaCollection);

        /// <summary>
        /// Store a collection of media passed as IEnumerable.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <param name="mediaCollection">IEnumerable of IMedia contract.</param>
        /// <returns>Task.Completed unless exception is thrown.</returns> 
        Task StashMedia(string path, string storageContainer, IEnumerable<IMedia> mediaCollection);

        /// <summary>
        /// Create and return media container found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <returns>Concrete IMediaContainer implementation <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IMediaContainer> GetMediaContainer(string path);

        /// <summary>
        /// Create and return media container found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <returns>Concrete IMediaContainer implementation <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IMediaContainer> GetMediaContainer(string path, string storageContainer);

        /// <summary>
        /// Return IEnumerable of IMedia found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <returns>Collection of IMedia <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IEnumerable<IMedia>> GetMedia(string path);

        /// <summary>
        /// Return IEnumerable of IMedia found at passed path.
        /// </summary>
        /// <param name="path">Path travelled from root container.</param>
        /// <param name="storageContainer">Overrides configuration root container.</param>
        /// <returns>Collection of IMedia <see cref="Models.MemoryStreamMedia"/></returns>
        Task<IEnumerable<IMedia>> GetMedia(string path, string storageContainer);
    }
}