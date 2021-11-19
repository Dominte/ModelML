using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ML;
using FIIServer.Models;

namespace FIIServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ModelController> _logger;
        private readonly PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> _predictionEnginePool;
        private readonly string _imagesTmpFolder;


        public ModelController(ILogger<ModelController> logger, PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> predictionEnginePool)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _imagesTmpFolder = GetAbsolutePath(@"ImagesTemp");
        }

        [HttpGet]
        public MLModel.ModelOutput Get(string url)
        {
            url = "https://upload.wikimedia.org/wikipedia/en/thumb/1/17/Bugs_Bunny.svg/1200px-Bugs_Bunny.svg.png";
            //    WebClient client = new WebClient();
            //client.DownloadFile(url, _imagesTmpFolder + "//1");
            //string imageFileRelativePath = @"../../../assets" + url;
            //string imageFilePath = GetAbsolutePath(imageFileRelativePath);

            var x = Download(url, _imagesTmpFolder, "gigi");
            var xxx = _imagesTmpFolder + "\\" + "altTest.jpg";

            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = xxx
            };

            //Load model and predict output
            var result = _predictionEnginePool.Predict(sampleData);

            return result;
        }

        [HttpPost]
        public MLModel.ModelOutput Post([FromBody] AndroidRequest androidRequest)
        {
            url = "https://upload.wikimedia.org/wikipedia/en/thumb/1/17/Bugs_Bunny.svg/1200px-Bugs_Bunny.svg.png";
            //    WebClient client = new WebClient();
            //client.DownloadFile(url, _imagesTmpFolder + "//1");
            //string imageFileRelativePath = @"../../../assets" + url;
            //string imageFilePath = GetAbsolutePath(imageFileRelativePath);

            var x = Download(url, _imagesTmpFolder, "gigi");
            var xxx = _imagesTmpFolder + "\\" + "altTest.jpg";

            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = xxx
            };

            //Load model and predict output
            var result = _predictionEnginePool.Predict(sampleData);

            return result;
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }

        public static bool Download(string url, string destDir, string destFileName)
        {
            if (destFileName == null)
                destFileName = url.Split(Path.DirectorySeparatorChar).Last();

            Directory.CreateDirectory(destDir);

            string relativeFilePath = Path.Combine(destDir, destFileName);

            //if (File.Exists(relativeFilePath))
            //{
            //    Console.WriteLine($"{relativeFilePath} already exists.");
            //    return false;
            //}

            var wc = new WebClient();
            Console.WriteLine($"Downloading {relativeFilePath}");
            var download = Task.Run(() => wc.DownloadFile(url, relativeFilePath));
            while (!download.IsCompleted)
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }
            Console.WriteLine("");
            Console.WriteLine($"Downloaded {relativeFilePath}");

            return true;
        }
    }
}
