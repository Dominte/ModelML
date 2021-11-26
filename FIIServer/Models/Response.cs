using System.Collections.Generic;

namespace FIIServer.Models
{
    public class Response
    {
        public MLModel.ModelOutput Model { get; set; }

        public string Prediction { get; set; }
        
        public float Score { get; set; }
        public List<Error> Errors { get; set; }
    }
}
