using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCLWebAPI.Models
{
    public class VersionInformationApiModel
    {
        public string VersionNumber { get; set; }
        public string BuildNumber { get; set; }
        public DateTime Date { get; set; }
        public string DeployedDateInfo { get; set; }
        
    }
}