using FIIServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private float _lowerBound = (float)00.94;

        public ModelController(ILogger<ModelController> logger, PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> predictionEnginePool)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _imagesTmpFolder = GetAbsolutePath(@"ImagesTemp");
        }

        [HttpPut("lowerBound")]
        public IActionResult SetLowerBound([FromBody] AndroidRequest androidRequest)
        {
            try
            {
                _lowerBound = (float) androidRequest.LowerBound / 100;
            }
            catch
            {
                var error = new Error("Could not overwrite lowerBound");
                return BadRequest(error);
            }

            return Ok($"Set lowerBound to {_lowerBound}");
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

            System.Diagnostics.Debug.WriteLine("Initialise conversion from Base64");

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

            System.Diagnostics.Debug.WriteLine("Converted from Base64");


            var filePath = "Pictures\\" + Guid.NewGuid();
            var fullPath = filePath + "." + imageFormat.ToString().ToLower();

            SaveImage(image, filePath, imageFormat);

            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = fullPath
            };

            var result = _predictionEnginePool.Predict(sampleData);
            
            Array.Sort(result.Score);
            Array.Reverse(result.Score);

            if (result.Score[0] < _lowerBound)
            {
                System.Diagnostics.Debug.WriteLine("Could not determine picture");

                var error = new Error("Picture could not be determined. Please retake the picture from another angle");
                DeleteImageByPath(fullPath);

                return BadRequest(error);
            }

            System.Diagnostics.Debug.WriteLine(result.Score[0] + "% : " + result.Prediction);

            response.Prediction = result.Prediction;
            response.Score = result.Score[0];

            var testResponse = new AndroidResponse
            {
                Score = result.Score[0],
                Prediction = result.Prediction
            };

            DeleteImageByPath(fullPath);

            return Ok(testResponse);
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
