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

        private readonly ICryptoTransform _encryptor, _decryptor;

#if NET45
        private RijndaelManaged _rijndaelManaged = new RijndaelManaged
        {
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
#else
        private Aes _aes = Aes.Create();
#endif
        
        public EncryptionProvider(IEncryptionConfiguration config)
        {
            Config = config;

#if NET45
            _rijndaelManaged.Key = config.GetKeyBytes;
            _rijndaelManaged.GenerateIV();

            _encryptor = _rijndaelManaged.CreateEncryptor();
            _decryptor = _rijndaelManaged.CreateDecryptor();
#else
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Key = config.GetKeyBytes;
            _aes.GenerateIV();

            _encryptor = _aes.CreateEncryptor();
            _decryptor = _aes.CreateDecryptor();
#endif
            
        }

#region Generic Functionality

        public Task<byte[]> ProcessAsync(byte[] data)
        {
            return EncryptAsync(data);
        }

        public byte[] Process(byte[] data)
        {
            return Encrypt(data);
        }

        public Task<byte[]> ReverseAsync(byte[] data)
        {
            return DecryptAsync(data);
        }

        public byte[] Reverse(byte[] data)
        {
            return Decrypt(data);
        }

#endregion

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

        public byte[] Encrypt(byte[] data)
        {
            return EncryptAsync(data).GetAwaiter().GetResult();
        }

        public async Task<byte[]> EncryptAsync(byte[] data)
        {
            using (var encryptedStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, _encryptor, CryptoStreamMode.Write))
                {
                    await cryptoStream.WriteAsync(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();

                    return encryptedStream.ToArray();
                }
            }
        }

        public async Task EncryptAsync(IEnumerable<IMedia> mediaCollection)
        {
            foreach (var media in mediaCollection)
            {
                using (var encryptedStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, _encryptor, CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(media.Data, 0, media.Data.Length);
                        cryptoStream.FlushFinalBlock();


                        media.Name = $"{media.Name}{Config.EncryptionExtension}";
                        media.Data = encryptedStream.ToArray();
                    }
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

        public async Task DecryptAsync(IEnumerable<IMedia> mediaCollection)
        {
            foreach (var media in mediaCollection)
            {
                using (var encryptedStream = new MemoryStream(media.Data))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, _decryptor, CryptoStreamMode.Read))
                    {
                        var buffer = new byte[media.Data.Length];

                        await cryptoStream.ReadAsync(buffer, 0, buffer.Length);


                        media.Data = buffer;
                        media.Name = media.Name.Replace($"{Config.EncryptionExtension}", string.Empty);
                    }
                }
            }
        }

        public async Task<byte[]> DecryptAsync(byte[] encrypted)
        {
            var buffer = new byte[encrypted.Length];

            using (var encryptedStream = new MemoryStream(encrypted))
            {
                using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, _decryptor, CryptoStreamMode.Read))
                {
                    await cryptoStream.ReadAsync(buffer, 0, buffer.Length);
                }
            }

            return buffer;
        }

        public byte[] Decrypt(byte[] encrypted)
        {
            return DecryptAsync(encrypted).GetAwaiter().GetResult();
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
