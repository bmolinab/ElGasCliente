using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.Models
{
    public class ModelState
    {
        public List<string> Error { get; set; }
    }
    public class ErrorModel
    {
        public string Message { get; set; }
        public ModelState ModelState { get; set; }
    }
}
