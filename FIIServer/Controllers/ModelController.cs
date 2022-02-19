using FIIServer.Controllers.Utils;
using FIIServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.ML;
using System;
using System.Drawing;

namespace FIIServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> _predictionEnginePool;
        private float _lowerBound = (float)00.94;

        public ModelController(PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpPut("lowerBound")]
        public IActionResult SetLowerBound([FromBody] AndroidRequest androidRequest)
        {
            try
            {
                _lowerBound = (float)androidRequest.LowerBound / 100;
            }
            catch
            {
                var error = new Error("Could not overwrite lowerBound");
                return BadRequest(error);
            }

            return Ok($"Set lowerBound to {_lowerBound}");
        }

        [HttpPost]
        public IActionResult Post([FromBody] AndroidRequest androidRequest)
        {
            System.Diagnostics.Debug.WriteLine("Accessed POST /model route");

            System.Diagnostics.Debug.WriteLine("Initialise conversion from Base64");

            var response = new Response();
            var imageFormat = Helper.ReturnImageFormat(androidRequest.ImageFormat);

            Image image;
            try
            {
                image = Helper.ConvertBase64ToImage(androidRequest.Base64);
            }
            catch
            {
                var error = new Error("Given string is not Base64");
                return BadRequest(error);
            }

            System.Diagnostics.Debug.WriteLine("Converted from Base64");

            var filePath = "Pictures\\" + Guid.NewGuid();
            var fullPath = filePath + "." + imageFormat.ToString().ToLower();

            Helper.SaveImage(image, filePath, imageFormat);

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
                Helper.DeleteImageByPath(fullPath);

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

            Helper.DeleteImageByPath(fullPath);

            return Ok(testResponse);
        }
    }
}
