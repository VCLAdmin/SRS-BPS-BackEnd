using System;

namespace VCLWebAPI.Models.BPS
{
    public class GlassConfigurationApiModel
    {
        public Guid GlassConfigurationGuid { get; set; }
        public Guid ProblemGuid { get; set; }
        public string ConfigurationString { get; set; }

        public GlassConfigurationApiModel()
        {
            GlassConfigurationGuid = Guid.Empty;
            ProblemGuid = Guid.Empty;
            ConfigurationString = String.Empty;
        }
    }
}