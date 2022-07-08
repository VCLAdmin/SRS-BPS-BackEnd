using BpsUnifiedModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCLWebAPI.Models.SRS
{
    public class SRSUnifiedApiModel
    {
        public string TemplateFileName { get; set; }
        public BpsUnifiedModel BPSUnifiedModel { get; set; }
    }
}