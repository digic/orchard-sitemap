using Digic.Sitemap.Models;
using Orchard;
using System.Collections.Generic;

namespace Digic.Sitemap.Providers
{
    public interface ISitemapRouteProvider : IDependency 
    {      
        IEnumerable<SitemapRoute> GetRoutes();
        int Priority { get; }
        
    }
}
