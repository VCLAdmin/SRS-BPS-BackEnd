using System.Collections.Generic;

namespace VCLWebAPI.Models
{
    public class FeatureApiModel
    {
        public int FeatureId { get; set; }
        public string FeatureGuid { get; set; }
        public string Feature { get; set; }
        public int ParentId { get; set; }
        public Permission Permission { get; set; }
    }
    public enum Permission
    {
        NoAccessc= 1,
        ReadOnly,
        WriteAccess,
        FullAccess
    }
}