# MediaStash
Fitcode image stash library helps easily leverage azure and amazon to store media files. With abstractions and providers for compression and encryption there is an extra layer of perfomance and security.

##Getting Started
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
