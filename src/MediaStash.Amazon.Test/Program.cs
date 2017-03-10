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

using Fitcode.MediaStash.AmazonStorage;
using Fitcode.MediaStash.Lib;
using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MediaStash.Amazon.Core.Test
{
    public class Program
    {
        private static IRepositoryConfiguration _repositoryConfiguration;
        private static IAmazonMediaRepository _mediaRepository;

        private static string _filename = "anime17.jpg";
        private static string _filePath = @"C:\Users\felip_kw0ekdh\Desktop\";
        private static string _amazonPath = "unit-test";

        public static void Main(string[] args)
        {
            Init();
            //TestListBucket();
            //TestUpload();
            //TestDownload();
            TestDirectioryUpload();

            Console.ReadKey();
        }

        private static void Init()
        {
            _repositoryConfiguration = new RepositoryConfiguration
            {
                RootContainer = "fitcode",
                Account = StorageConnection.Account
            };
            _mediaRepository = new MediaRepository(_repositoryConfiguration);
        }

        public static void TestDirectioryUpload()
        {
            _mediaRepository.OnDirectoryStash += (n) =>
            {
                Console.WriteLine($"Total Megs: {n.TotalMegabytes.ToString("f2")} Processed: {n.ProcessedMegabytes.ToString("f2")}");
            };

            _mediaRepository.StashDirectoryAsync(@"E:\azure-test", true).Wait();
        }

        private static void TestUpload()
        {
            var container = new MediaContainer
            {
                Media = new List<GenericMedia>
                {
                    new GenericMedia(_filename, new FileStream($"{_filePath}{_filename}", FileMode.Open).ToByteArray(true))
                }
            };

            _mediaRepository.StashMediaAsync(_amazonPath, container.Media).Wait();
        }

        private static void TestDownload()
        {
            var result = _mediaRepository.GetMediaAsync(_amazonPath, true).GetAwaiter().GetResult();
            foreach (var media in result)
            {
                
            }
        }

        private static void TestListBucket()
        {
            var content = _mediaRepository.ListObjectRequest(_repositoryConfiguration.RootContainer, "unit-test").Result;
            foreach(var item in content)
            {
                Console.WriteLine(item.Key);
            }
        }
    }
}
