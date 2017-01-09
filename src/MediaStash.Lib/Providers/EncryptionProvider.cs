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
using System.Security.Cryptography;
using Fitcode.MediaStash.Lib;

namespace Fitcode.MediaStash.Lib.Providers
{
    public class EncryptionProvider : IEncryptionProvider
    {

       public IEncryptionConfiguration Config { get; private set; }

        private RijndaelManaged _rijndaelManaged = new RijndaelManaged
        {
            Mode = CipherMode.ECB
        };
        private readonly ICryptoTransform _encryptor, _decryptor;

        public EncryptionProvider(IEncryptionConfiguration config)
        {
            Config = config;

            _encryptor = _rijndaelManaged.CreateEncryptor(config.GetKeyBytes, new byte[] { });
            _decryptor = _rijndaelManaged.CreateDecryptor(config.GetKeyBytes, new byte[] { });
        }
        
        public void Encrypt(IEnumerable<IMedia> mediaCollection)
        {
            EncryptAsync(mediaCollection).Wait();
        }

        public void Encrypt(MemoryMediaContainer memoryMediaContainer)
        {
            EncryptAsync(memoryMediaContainer.Media).Wait();
        }

        public void Encrypt(MediaContainer mediaContainer)
        {
            EncryptAsync(mediaContainer.Media).Wait();
        }

        public async Task EncryptAsync(IEnumerable<IMedia> mediaCollection)
        {
            foreach (var media in mediaCollection) {
                using (var encryptedStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, _encryptor, CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(media.Data, 0, media.Data.Length);
                    }

                    media.Name = $"{media.Name}.{Config.EncryptionExtension}";
                    media.Data = encryptedStream.ToByteArray();
                }
            }
        }

        public async Task EncryptAsync(MemoryMediaContainer memoryMediaContainer)
        {
            await EncryptAsync(memoryMediaContainer.Media);
        }

        public async Task EncryptAsync(MediaContainer mediaContainer)
        {
            await EncryptAsync(mediaContainer.Media);
        }

        public async Task DecryptAsync(MediaContainer mediaContainer)
        {
            await DecryptAsync(mediaContainer.Media);
        }

        public Task DecryptAsync(IEnumerable<IMedia> mediaCollection)
        {
            foreach (var media in mediaCollection)
            {
                using (var encryptedStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, _encryptor, CryptoStreamMode.Read))
                    {
                        media.Data = cryptoStream.ToByteArray();
                        media.Name = media.Name.Replace($".{Config.EncryptionExtension}", string.Empty);
                    }
                }
            }

            return Task.FromResult(0);
        }

        public void Decrypt(MediaContainer mediaContainer)
        {
            DecryptAsync(mediaContainer.Media).Wait();
        }

        public void Decrypt(IEnumerable<IMedia> media)
        {
            DecryptAsync(media).Wait();
        }
    }
}
