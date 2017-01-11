using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fitcode.MediaStash.Lib.Abstractions;
using Microsoft.AspNetCore.Http;
using System.IO;
using Fitcode.MediaStash.Lib.Models;

namespace SampleWebApp.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediaRepository _mediaRepository;

        public HomeController(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            var mediaCollection = new List<GenericMedia>(files.Count);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream()) {
                        await formFile.CopyToAsync(memoryStream);

                        mediaCollection.Add(new GenericMedia(Path.GetFileName(formFile.FileName), memoryStream.ToArray()));
                    }
                }
            }

            if (mediaCollection.Count > 0)
                await _mediaRepository.StashMediaAsync("getting-started", mediaCollection);

            return Ok(new { count = files.Count, size, filePath });
        }

        public async Task<ActionResult> ViewFromFiles()
        {
            var result = await _mediaRepository.GetMediaAsync("getting-started");

            return View(result);
        }

        public async Task<ActionResult> ViewFromPublicUrl()
        {
            var result = await _mediaRepository.GetMediaAsync("getting-started", true);

            return View(result);
        }
    }
}
