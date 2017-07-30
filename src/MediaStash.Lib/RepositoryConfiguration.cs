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

using MediaStash.Lib;
using MediaStash.Lib.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Fitcode.MediaStash.Lib
{
    public class RepositoryConfiguration : IRepositoryConfiguration
    {
        public ServiceAccount Account { get; set; }

        public string ConnectionString { get; set; }

        public string RootContainer { get; set; }
    }

    public class CompressionConfiguration : ICompressionConfiguration
    {
        public IEnumerable<string> SupportedExtensions { get; set; } = new List<string>
        {
             ".png", ".gif", ".bmp", ".jpg", ".avi", ".mp4", ".flv"
        };
    }

    public class EncryptionConfiguration : IEncryptionConfiguration
    {
        public string Password { get; set; }

        public string Salt { get; set; }

        private Rfc2898DeriveBytes _passwordDeriveBytes = null;
        public Rfc2898DeriveBytes PasswordDeriveBytes
        {
            get
            {
                if (_passwordDeriveBytes == null)
                {
                    byte[] salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    if (!string.IsNullOrEmpty(Salt))
                        salt = Encoding.ASCII.GetBytes(Salt);

                    _passwordDeriveBytes = new Rfc2898DeriveBytes(Password, salt);
                }

                return _passwordDeriveBytes;
            }
        }

        public byte[] GetKeyBytes
        {
            get
            {
                return PasswordDeriveBytes.GetBytes(256 / 8);
            }
        }

        public string EncryptionExtension { get; set; } = ".encrypted";

        public void Reset(string password, string salt)
        {
            this.Password = password;
            this.Salt = salt;

            this._passwordDeriveBytes = null;
        }
    }
}
