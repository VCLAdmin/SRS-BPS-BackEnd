using System;
using System.Collections.Generic;

namespace VCLWebAPI.Models.Systems
{
    public class ProductApiModel
    {
        public int ProductId { get; set; }
        public Nullable<System.Guid> ProductGuid { get; set; }
        public string Name { get; set; }
        public string PrettyName { get; set; }
        public List<ArticleApiModel> Article { get; set; }
    }
}