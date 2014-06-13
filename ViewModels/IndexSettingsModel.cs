using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digic.Sitemap.ViewModels
{
    public class IndexSettingsModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IndexForDisplay { get; set; }
        public bool IndexForXml { get; set; }
        public string UpdateFrequency { get; set; }
        public int Priority { get; set; }
    }

    public class ContentTypeNameComparer : IEqualityComparer<IndexSettingsModel>
    {
        public bool Equals(IndexSettingsModel model1, IndexSettingsModel model2)
        {
            return model1.Name.Equals(model2.Name); 
        }

        public int GetHashCode(IndexSettingsModel model)
        {
             return model.Name.GetHashCode();
        }
    }

}