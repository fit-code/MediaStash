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
using Fitcode.MediaStash.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fitcode.MediaStash.Lib.Models
{
    /// <summary>
    /// GenericMedia stores media data at the lowest form.
    /// </summary>
    public class GenericMedia : IMedia
    {
        private string _mime = null;

        public byte[] Data { get; set; }

        public string Name { get; set; }
        public string Uri { get; set; }

        public string Mime
        {
            get
            {
                if (_mime == null)
                    _mime = MimeResolver.GetMimes(Path.GetExtension(Name)).FirstOrDefault();

                return _mime;
            }
            set { _mime = value; }
        }

        public Dictionary<string, string> Metadata { get; set; }

        public GenericMedia(string name, byte[] data)
        {
            this.Name = name;
            this.Data = data;
        }

        public GenericMedia(string name, Stream stream) : this(name, stream.ToByteArray(true))
        {

        }

        public string ToBase64String()
        {
            if (Data == null)
                return null;

            return Convert.ToBase64String(Data);
        }
    }
}
