using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digic.Sitemap.ViewModels
{
    public class IndexingViewModel
    {
        public List<IndexSettingsModel> ContentTypeSettings { get; set; }
        public List<CustomRouteModel> CustomRoutes { get; set; }
    }
}