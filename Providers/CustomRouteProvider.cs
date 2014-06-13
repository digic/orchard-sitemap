using Digic.Sitemap.Models;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Digic.Sitemap.Extensions;

namespace Digic.Sitemap.Providers
{
    public class CustomRouteProvider : ISitemapRouteProvider
    {
        private readonly IRepository<SitemapCustomRouteRecord> _customRoutes;

        public CustomRouteProvider(IRepository<SitemapCustomRouteRecord> customRoutes)
        {
            _customRoutes = customRoutes;
        }


        public IEnumerable<SitemapRoute> GetRoutes()
        {
            return _customRoutes.Table              
               .Select(r => new SitemapRoute
               {
                   Url = r.Url,
                   Title = r.Url.UrlToTitle(),
                   Priority = r.Priority,
                   UpdateFrequency = r.UpdateFrequency
               })
               .AsEnumerable();
        }

        public int Priority
        {
            get { return 100; }
        }
    }
}