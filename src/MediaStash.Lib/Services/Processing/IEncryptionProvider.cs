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

using MediaStash.Lib.Configs;
using MediaStash.Lib.Models;
using MediaStash.Lib.Services.Content;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaStash.Lib.Services.Processing
{
    public interface IEncryptionProvider : IProcessProvider
    {
        IEncryptionConfiguration Config { get; }

        /// <summary>
        /// Generic encryption for byte[] for alternative file sources.
        /// </summary>
        /// <param name="data">byte[]</param>
        /// <returns>encrypted byte[]</returns>
        Task<byte[]> EncryptAsync(byte[] data);

        Task EncryptAsync(MediaContainer mediaContainer);
        Task EncryptAsync(MemoryMediaContainer memoryMediaContainer);
        Task EncryptAsync(IEnumerable<IMedia> mediaCollection);

        /// <summary>
        /// Generic encryption for byte[] for alternative file sources.
        /// </summary>
        /// <param name="data">byte[]</param>
        /// <returns>encrypted byte[]</returns>
        byte[] Encrypt(byte[] data);

        void Encrypt(MediaContainer mediaContainer);
        void Encrypt(MemoryMediaContainer memoryMediaContainer);
        void Encrypt(IEnumerable<IMedia> mediaCollection);

        /// <summary>
        /// Generic decryptor with no concept of type.
        /// </summary>
        /// <param name="data">encrypted byte[]</param>
        /// <returns>decrypted byte[]</returns>
        Task<byte[]> DecryptAsync(byte[] data);
        Task DecryptAsync(MediaContainer mediaContainer);
        Task DecryptAsync(IEnumerable<IMedia> mediaCollection);

        /// <summary>
        /// Generic decryptor with no concept of type.
        /// </summary>
        /// <param name="data">encrypted byte[]</param>
        /// <returns>decrypted byte[]</returns>
        byte[] Decrypt(byte[] data);

        void Decrypt(MediaContainer mediaContainer);
        void Decrypt(IEnumerable<IMedia> mediaCollection);

    }
}
