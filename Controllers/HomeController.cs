using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using ImageAnalysis.Models;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
//https://codelabs.developers.google.com/codelabs/cloud-vision-api-csharp/#3

namespace ImageAnalysis.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            var url = @"C:\src\ImageAnalysis\key.json";
            var credential = GoogleCredential.FromFile(url);
            var file = files.FirstOrDefault();
            var stream = file?.OpenReadStream();
            var channel = new Grpc.Core.Channel(ImageAnnotatorClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
            var client = ImageAnnotatorClient.Create(channel);
            var image = await Image.FromStreamAsync(stream);
            var props = await client.DetectLabelsAsync(image);
            var json = JsonConvert.SerializeObject(props);
            return View(json);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
