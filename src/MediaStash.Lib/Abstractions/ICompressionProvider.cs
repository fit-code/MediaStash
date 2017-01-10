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

using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fitcode.MediaStash.Lib.Abstractions
{
    public interface ICompressionProvider
    {
        ICompressionConfiguration Config { get; }

        Task<CompressedPack> PackAsync(string name, MediaContainer mediaContainer);
        Task<CompressedPack> PackAsync(string name, MemoryMediaContainer memoryMediaContainer);
        Task<CompressedPack> PackAsync(string name, IEnumerable<IMedia> mediaCollection);

        CompressedPack Pack(string name, MediaContainer mediaContainer);
        CompressedPack Pack(string name, MemoryMediaContainer memoryMediaContainer);
        CompressedPack Pack(string name, IEnumerable<IMedia> mediaCollection);

        Task<MediaContainer> UnpackAsync(byte[] buffer);
        Task<MediaContainer> UnpackAsync(MemoryStream stream);

        MediaContainer Unpack(byte[] buffer);
        MediaContainer Unpack(MemoryStream stream);

    }
}
