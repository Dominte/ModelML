using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIIServer.Models
{
    public class Error
    {
        public string ErrorMessage { get; set; }

        public Error(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
