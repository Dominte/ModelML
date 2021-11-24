using System.Collections.Generic;

namespace FIIServer.Models
{
    public class Response
    {
        public MLModel.ModelOutput Model { get; set; }

        public List<Error> Errors { get; set; }
    }
}
