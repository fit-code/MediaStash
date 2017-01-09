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
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Fitcode.MediaStash.Lib.Models
{
    /// <summary>
    /// Basic outline for media files.
    /// </summary>
    /// <typeparam name="T">Media type template constrained to Stream.</typeparam>
    public abstract class MediaBase<T> : IMedia, IDisposable where T : Stream
    {
        public string Name { get; set; }

        public T Media { get; set; }

        public string Uri { get; set; }

        public virtual byte[] Data
        {
            get
            {
                return Media?.ToByteArray();
            }set
            {
                Media.SetLength(value.Length);
                Media.Write(value, 0, value.Length);
            }
        }

        public MediaBase(string name, T media)
        {
            this.Name = name;
            this.Media = media;
        }

        public string ToBase64String()
        {
            if (Data == null)
                return null;

            return Convert.ToBase64String(Data);
        }

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_disposed) return;

            if (isDisposing)
            {
                _disposed = true;

                if (Media != null)
                {
                    Media.Dispose();
                    Media = null;
                }
            }
        }
    }
}
