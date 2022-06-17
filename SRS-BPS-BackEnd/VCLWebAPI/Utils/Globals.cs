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
        public static string ConnectionString { get; set; }
        public static string ConnectionStringApp { get; set; }
        public static string Issuer { get; set; }
        public static string Audience { get; set; }
        public static string Secret { get; set; }

    }
}