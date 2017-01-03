#region License
// Copyright (c) 2017 Fitcode.io
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

using PCLStorage;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Fitcode.MediaStash.Lib.Models
{
    /// <summary>
    /// MemoryStreamFile wraps a MemoryStream to enforce the IFile constraint at the MediaBase<T>
    /// </summary>
    public class MemoryStreamFile : IFile
    {
        public MemoryStreamFile(MemoryStream stream)
        {
            if (stream == null)
                throw new InvalidOperationException("Unable to create MemoryStreamFile with null stream.");

            Stream = stream;
        }

        public MemoryStream Stream { get; private set; }
        public string Name { get; private set; }

        public string Path { get; set; }

        public Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Stream.Dispose();
            Stream = null;

            return Task.FromResult(0);
        }

        public Task MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new InvalidOperationException("Unable to move MemoryStream.");
        }

        public Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Stream as Stream);
        }

        public Task RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new InvalidOperationException("Unable to rename MemoryStream.");
        }
    }
}
