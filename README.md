# MediaStash
Fitcode image stash library helps easily leverage azure and amazon to store media files.

```c#
            var config = new RepositoryConfiguration
            {
                ConnectionString = "connectionString",
                RootContainer = "dev"
            };
            var mediaRepository = new MediaRepository(config);

            var path = $@"{Guid.NewGuid()}";

            _mediaRepository.StashMedia(path, new List<FileStreamMedia>
            {
                new FileStreamMedia("anime16.jpg",new FileStream(@"Desktop\anime16.jpg", FileMode.Open))
            }).Wait();

            var media = _mediaRepository.GetMedia(path).Result;

            Console.ReadKey(); 
```

# Contract

```c#
    public interface IMediaRepository
    {
        IRepositoryConfiguration Config { get; }

        Task StashContainer(IMediaContainer mediaContainer);
        Task StashContainer(IMediaContainer mediaContainer, string storageContainer);
        Task StashMedia(string path, IEnumerable<IMedia> mediaCollection);
        Task StashMedia(string path, string storageContainer, IEnumerable<IMedia> mediaCollection);
        
        Task<IMediaContainer> GetMediaContainer(string path);
        Task<IMediaContainer> GetMediaContainer(string path, string storageContainer);
        Task<IEnumerable<IMedia>> GetMedia(string path);
        Task<IEnumerable<IMedia>> GetMedia(string path, string storageContainer);
    }
```
