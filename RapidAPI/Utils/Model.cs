using System;
using System.Collections.Generic;
using System.Text;

namespace RapidAPI.Utils
{
    public class Model
    {
        public string country { get; set; }
        public string code { get; set; }
        public string confirmed { get; set; }
        public string recovered { get; set; }
        public string critical { get; set; }
        public string deaths { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string lastChange { get; set; }
        public string lastUpdate { get; set; }
    }
}
