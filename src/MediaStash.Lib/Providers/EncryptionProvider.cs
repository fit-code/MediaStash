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

using MediaStash.Lib.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using System.IO;
using System.Security.Cryptography;

namespace MediaStash.Lib.Providers
{
    public class EncryptionProvider : IEncryptionProvider
    {

       public IEncryptionConfiguration Config { get; private set; }

        public EncryptionProvider(IEncryptionConfiguration config)
        {
            Config = config;
        }

        public MediaContainer Decrypt(MemoryStream stream)
        {
            throw new NotImplementedException();
        }

        public MediaContainer Decrypt(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public Task<MediaContainer> DecryptAsync(MemoryStream stream)
        {
            //string password = @"myKey123"; // Your Key Here

            //UnicodeEncoding UE = new UnicodeEncoding();
            //byte[] key = UE.GetBytes(password);

            //FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

            //RijndaelManaged RMCrypto = new RijndaelManaged();

            //CryptoStream cs = new CryptoStream(fsCrypt,
            //    RMCrypto.CreateDecryptor(key, key),
            //    CryptoStreamMode.Read);

            //FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            //int data;
            //while ((data = cs.ReadByte()) != -1)
            //    fsOut.WriteByte((byte)data);

            //fsOut.Close();
            //cs.Close();
            //fsCrypt.Close();






            //// Initialize Rijndael key object.
            //RijndaelManaged symmetricKey = new RijndaelManaged();

            //// If we do not have initialization vector, we cannot use the CBC mode.
            //// The only alternative is the ECB mode (which is not as good).
            //if (initVectorBytes.Length == 0)
            //    symmetricKey.Mode = CipherMode.ECB;
            //else
            //    symmetricKey.Mode = CipherMode.CBC;

            //// Create encryptor and decryptor, which we will use for cryptographic
            //// operations.
            //encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            //decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            throw new NotImplementedException();
        }

        public Task<MediaContainer> DecryptAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Encrypt(IEnumerable<IMedia> mediaCollection)
        {
            throw new NotImplementedException();
        }

        public void Encrypt(MemoryMediaContainer memoryMediaContainer)
        {
            throw new NotImplementedException();
        }

        public void Encrypt(MediaContainer mediaContainer)
        {
            throw new NotImplementedException();
        }

        public Task EncryptAsync(IEnumerable<IMedia> mediaCollection)
        {
            
                //string password = @"myKey123"; // Your Key Here
                //UnicodeEncoding UE = new UnicodeEncoding();
                //byte[] key = UE.GetBytes(password);

                //string cryptFile = outputFile;
                //FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                //RijndaelManaged RMCrypto = new RijndaelManaged();

                //CryptoStream cs = new CryptoStream(fsCrypt,
                //    RMCrypto.CreateEncryptor(key, key),
                //    CryptoStreamMode.Write);

                //FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                //int data;
                //while ((data = fsIn.ReadByte()) != -1)
                //    cs.WriteByte((byte)data);


                //fsIn.Close();
                //cs.Close();
                //fsCrypt.Close();
            

            throw new NotImplementedException();
        }

        public Task EncryptAsync(MemoryMediaContainer memoryMediaContainer)
        {
            throw new NotImplementedException();
        }

        public Task EncryptAsync(MediaContainer mediaContainer)
        {
            throw new NotImplementedException();
        }
    }
}
