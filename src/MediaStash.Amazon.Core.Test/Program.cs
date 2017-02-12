using Fitcode.MediaStash.AmazonStorage;
using Fitcode.MediaStash.Lib;
using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using MediaStash.Amazon.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            TestListBucket();
            TestDirectioryUpload();
            //TestUpload();
            TestDownload();
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
            var result = _mediaRepository.GetMediaAsync(_amazonPath, true).Result;
            foreach (var media in result)
            {

            }
        }

        private static void TestListBucket()
        {
            var content = _mediaRepository.ListObjectRequest(_repositoryConfiguration.RootContainer, "unit-test").Result;
            foreach (var item in content)
            {
                Console.WriteLine(item.Key);
            }
        }
    }
}
