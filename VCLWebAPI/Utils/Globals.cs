using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCLWebAPI.Utils
{
    public class Constants
    {
    }

    public static class Globals
    {
        public static string VCLDesignDBConnection { get; set; }
        public static string ApplicationDBConnection { get; set; }
        public static string Issuer { get; set; }
        public static string Audience { get; set; }
        public static string Secret { get; set; }
        public static string StartupAssembly { get; set; }
        public static string accessKey { get; set; }
        public static string secretKey { get; set; }
        public static string service_url { get; set; }
        public static string bucket_name { get; set; }
        public static string SENDGRID_API_KEY { get; set; }


    }
}