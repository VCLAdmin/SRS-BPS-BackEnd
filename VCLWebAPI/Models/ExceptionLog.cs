using System;
using System.Collections.Generic;

namespace VCLWebAPI.Models
{
    public class ExceptionLog
    {
        public long ExceptionLogId { get; set; }
        public System.Guid ExceptionLogExternalId { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ErrorCode { get; set; }
        public string Exception { get; set; }
        public string URI { get; set; }
        public string ExceptionDetail { get; set; }
        public string InnerException { get; set; }
        public string RequestUrl { get; set; }
        public string RequestUrlReferrer { get; set; }
        public string RequestUserAgent { get; set; }
    }
}