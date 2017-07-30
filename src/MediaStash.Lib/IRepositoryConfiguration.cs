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

using MediaStash.Lib.Models;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MediaStash.Lib
{
    public interface IRepositoryConfiguration
    {
        string ConnectionString { get; }
        
        string RootContainer { get; }

        ServiceAccount Account { get; }
    }

    public interface ICompressionConfiguration
    {
        IEnumerable<string> SupportedExtensions { get; }
    }

    public interface IEncryptionConfiguration
    {
        string EncryptionExtension { get; }
        string Password { get; }
        string Salt { get; }
        Rfc2898DeriveBytes PasswordDeriveBytes { get; }
        byte[] GetKeyBytes { get; }
        void Reset(string password, string salt);
    }
}
