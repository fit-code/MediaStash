# MediaStash
MediaStash library helps easily leverage azure and amazon to store media files. With abstractions and providers for compression and encryption there is an extra layer of perfomance and security.

Let's say you want to encrypt your images upstream/downstream; with MediaStash you can do it with the default provider, or by adding your own custom implementation. You can also chain providers ex: you can encrypt+watermark by simply adding multiple providers.

## Getting Started
Take a look at our getting started solution for a fully functionality MVC application.

## Sample Upload 
```c#
            var container = new MediaContainer
            {
                Media = new List<GenericMedia>
                {
                    new GenericMedia(_filename, new FileStream($"{_filePath}{_filename}", FileMode.Open).ToByteArray(true)),
                    new GenericMedia(_filename, new FileStream($"{_filePath}{_filename}", FileMode.Open).ToByteArray(true)),
                    new GenericMedia(_filename, new FileStream($"{_filePath}{_filename}", FileMode.Open).ToByteArray(true))
                }
            };

            _mediaRepository.StashMediaAsync(_azurePath, container.Media).Wait(); 
```

## Sample Download 
```c#
   // Download file and returns public url.
   var result = _mediaRepository.GetMediaAsync(_azurePath).Result;
   
   // Just public url.
   var result = _mediaRepository.GetMediaAsync(_azurePath, true).Result;
```

## Directory Upload 
```c#
            // Registering to this event will received status of directory upload.
            _mediaRepository.OnDirectoryStash += (n) =>
            {
                Console.WriteLine($"Total Megs: {n.TotalMegabytes.ToString("f2")} Processed: {n.ProcessedMegabytes.ToString("f2")}");
            };
            _mediaRepository.StashDirectoryAsync($"{_dirpath}", true).Wait();
```
