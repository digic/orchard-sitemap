using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digic.Sitemap.Filters
{
    public class ContentPermissionFilter : ISitemapRouteFilter
    {
        public bool AllowUrl(string path)
        {
            return true;
        }
    }
}