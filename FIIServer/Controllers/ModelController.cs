﻿using FIIServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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
        private string folderPath = "ImagesTemp";

        public ModelController(ILogger<ModelController> logger, PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> predictionEnginePool)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _imagesTmpFolder = GetAbsolutePath(@"ImagesTemp");
        }

        [HttpGet]
        public void Retrain()
        {
            MLModel.RetrainPipeline()
        }

        [HttpGet]
        public MLModel.ModelOutput Get()
        {
            System.Diagnostics.Debug.WriteLine("Accessed GET /model");

            var xxx = _imagesTmpFolder + "\\" + "altTest.jpg";


            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = xxx
            };

            //Load model and predict output
            var result = _predictionEnginePool.Predict(sampleData);

            System.Diagnostics.Debug.WriteLine(result + " : " + result.Prediction);

            return result;
        }

        [HttpPost]
        public MLModel.ModelOutput Post([FromBody] AndroidRequest androidRequest)
        {
            //    WebClient client = new WebClient();
            //client.DownloadFile(url, _imagesTmpFolder + "//1");
            //string imageFileRelativePath = @"../../../assets" + url;
            //string imageFilePath = GetAbsolutePath(imageFileRelativePath);

            var image = ConvertBase64ToImage(androidRequest.Base64);

            var filePath = "Pictures\\" + Guid.NewGuid().ToString();

            SaveImage(image, filePath, ImageFormat.Png);

                var fullPath = filePath + "." + ImageFormat.Png.ToString();

            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = fullPath
            };

            //Load model and predict output
            var result = _predictionEnginePool.Predict(sampleData);

            DeleteImageByPath(fullPath);

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

        public Image ConvertBase64ToImage(string base64) => (Bitmap)new ImageConverter().ConvertFrom(Convert.FromBase64String(base64));

        public static bool SaveImage(Image image, string filepath, ImageFormat imageFormat)
        {
            try
            {
                image.Save(filepath + ".png", imageFormat);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteImageByPath(string path)
        {
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            return false;
        }
    }
}
