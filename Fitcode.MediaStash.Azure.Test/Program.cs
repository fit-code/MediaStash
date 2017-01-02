using Fitcode.MediaStash.Lib;
using Fitcode.MediaStash.Lib.Contracts;
using Fitcode.MediaStash.Lib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fitcode.MediaStash.Azure.Test
{
    public class Program
    {
        private static IRepositoryConfiguration _config = null;
        private static IMediaRepository _mediaRepository = null;


        public static void Main(string[] args)
        {
            _config = new RepositoryConfiguration
            {
                ConnectionString = File.ReadAllText("storage_connection.txt"),
                RootContainer = "dev"
            };
            _mediaRepository = new MediaRepository(_config);

            var id = Guid.NewGuid();

            _mediaRepository.StashMedia($@"{id}", new List<FileStreamMedia>
            {
                new FileStreamMedia("anime16.jpg",new FileStream(@"Desktop\anime16.jpg", FileMode.Open))
            }).Wait();

            var media = _mediaRepository.GetMedia($@"{id}").Result;

            Console.ReadKey();
        }
    }
}
