using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.classes
{


    public class DltTemplateResponse
    {
        public string code { get; set; }
        public string msg { get; set; }
        public dynamic data { get; set; }
    }
    public class DltTemplateResponseResult
    {
        public string message { get; set; }
        public string totalVerified { get; set; }
        public string totalPassed { get; set; }
        public string totalFailed { get; set; }
        public dynamic[] result { get; set; }
    }

}