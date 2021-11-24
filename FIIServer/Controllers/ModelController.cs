using FIIServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.String;

namespace FIIServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly ILogger<ModelController> _logger;
        private readonly PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> _predictionEnginePool;
        private readonly string _imagesTmpFolder;
        private readonly float _lowerBound = (float)0.75;

        public ModelController(ILogger<ModelController> logger, PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> predictionEnginePool)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _imagesTmpFolder = GetAbsolutePath(@"ImagesTemp");
        }

        [HttpGet]
        public IActionResult Get()
        {
            System.Diagnostics.Debug.WriteLine("Accessed GET /model route");
            var folder = _imagesTmpFolder + "\\" + "altTest.jpg";
            var response = new Response();

            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = folder
            };

            var result = _predictionEnginePool.Predict(sampleData);
            Array.Sort(result.Score);
            Array.Reverse(result.Score);

            if (result.Score[0] < _lowerBound)
            {
                var error = new Error("Picture could not be determined clearly. Please retake the picture from another angle");
                return NoContent();
            }

            System.Diagnostics.Debug.WriteLine(result.Score[0] + "% : " + result.Prediction);
            response.Model = result;

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AndroidRequest androidRequest)
        {
            System.Diagnostics.Debug.WriteLine("Accessed POST /model route");

            var response = new Response();
            var imageFormat = ReturnImageFormat(androidRequest.ImageFormat);

            Image image;
            try
            {
                image = ConvertBase64ToImage(androidRequest.Base64);
            }
            catch
            {
                var error = new Error("Given string is not Base64");
                return BadRequest(error);
            }

            var filePath = "Pictures\\" + Guid.NewGuid();
            var fullPath = filePath + "." + imageFormat.ToString().ToLower();

            SaveImage(image, filePath, imageFormat);

            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = fullPath
            };

            response.Model = _predictionEnginePool.Predict(sampleData);

            DeleteImageByPath(fullPath);

            return Ok(response);
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
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
            var file = new FileInfo(path);
            if (!file.Exists) return false;
            file.Delete();
            return true;
        }

        public static ImageFormat ReturnImageFormat(string imageFormat)
        {
            if (IsNullOrEmpty(imageFormat) || imageFormat.ToLower().Equals("png"))
            {
                return ImageFormat.Png;
            }

            if (imageFormat.ToLower().Equals("jpg") || imageFormat.ToLower().Equals("jpeg"))
            {
                return ImageFormat.Jpeg;
            }

            return ImageFormat.Png;
        }
    }
}
