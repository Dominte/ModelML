using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIIServer.Models
{
    public class TestResponse
    {
        public string Prediction { get; set; }

        public float Score { get; set; }

        public string ErrorMessage { get; set; }
    }
}
