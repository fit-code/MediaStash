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

using Fitcode.MediaStash.Azure;
using Fitcode.MediaStash.Lib;
using Fitcode.MediaStash.Lib.Providers;
using MediaStash.Lib.Models;
using MediaStash.Lib.Services.Content;
using MediaStash.Lib.Services.Processing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MediaStash.Lib.Test
{
    [TestFixture]
    public class AzureIntegrationTest
    {
        private static IRepositoryConfiguration _repositoryConfiguration;
        private static IMediaRepository _mediaRepository;

        private static string _filename = "anime.jpg";
        private static string _filePath = @"C:\Users\felip\Desktop\";
        private static string _azurePath = "unit-test";

        [SetUp]
        public void Init()
        {
            _repositoryConfiguration = new RepositoryConfiguration
            {
                RootContainer = "dev",
                ConnectionString = Azure.Test.StorageConnection.ConnectionString
            };
            _mediaRepository = new MediaRepository(_repositoryConfiguration,
                new List<IProvider>
                {
                    new EncryptionProvider(new EncryptionConfiguration {
                        Password = "test",
                        EncryptionExtension = ".sec",
                    })
                });
        }

        [TearDown]
        public void Cleanup()
        {

        }

        [Test]
        public static void TestDirectioryUpload()
        {
            _mediaRepository.OnDirectoryStash += (n) =>
            {
                Debug.WriteLine($"Total Megs: {n.TotalMegabytes.ToString("f2")} Processed: {n.ProcessedMegabytes.ToString("f2")}");
            };

            _mediaRepository.StashDirectoryAsync($@"{_filePath}Test", true).Wait();         
        }

        [Test]
        public static void TestMediaUpload()
        {
            var container = new MediaContainer
            {
                Media = new List<GenericMedia>
                {
                    new GenericMedia(_filename, new FileStream($"{_filePath}{_filename}", FileMode.Open).ToByteArray(true))
                }
            };

            _mediaRepository.StashMediaAsync(_azurePath, container.Media).Wait();
           
        }

        [Test]
        public static void TestMediaDownload()
        {
            var result = _mediaRepository.GetMediaAsync(_azurePath, false).Result;
            foreach (var media in result)
            {
                using (var writer = new FileStream($@"{_filePath}Test\{media.Name}", FileMode.Create))
                {
                    writer.Write(media.Data, 0, media.Data.Length);
                }  
            }
        }
    }
}
