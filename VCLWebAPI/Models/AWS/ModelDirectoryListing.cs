using System.Collections.Generic;

namespace VCLWebAPI.Models.AWS
{
    public class ModelDirectoryListing
    {
        public List<string> Skysphere { get; set; }
        public List<string> Thermals { get; set; }
        public List<string> Textures { get; set; }
        public List<string> Model { get; set; }
        public List<string> ModelData { get; set; }

        public ModelDirectoryListing()
        {
            Skysphere = new List<string>();
            Thermals = new List<string>();
            Textures = new List<string>();
            Model = new List<string>();
            ModelData = new List<string>();
        }
    }
}