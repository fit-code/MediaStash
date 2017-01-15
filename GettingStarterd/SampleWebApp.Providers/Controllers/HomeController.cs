using Fitcode.MediaStash.Lib.Abstractions;
using Fitcode.MediaStash.Lib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SampleWebApp.Providers.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly string _container = "getting-started-providers";

        public HomeController(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }
       
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ViewFromFiles()
        {
            var result = await _mediaRepository.GetMediaAsync(_container);

            return View(result);
        }

        public async Task<ActionResult> ViewFromPublicUrl()
        {
            var result = await _mediaRepository.GetMediaAsync(_container, true);

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    var mediaCollection = new List<GenericMedia>
                    {
                        new GenericMedia(Path.GetFileName(file.FileName), file.InputStream)
                    };

                    await _mediaRepository.StashMediaAsync(_container, mediaCollection);

                    
                    ViewBag.Message = "File uploaded successfully";
                    ViewBag.UploadPath = mediaCollection.FirstOrDefault().Uri;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            return View();
        }
        
    }
}