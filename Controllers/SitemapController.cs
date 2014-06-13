using Digic.Sitemap.Services;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Services;
using Orchard.Settings;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Digic.Sitemap.Controllers
{
    
    public class SitemapController : Controller
    {
        private readonly ISitemapService _sitemapService;
        private readonly ICacheManager _cacheManager;
        private readonly IClock _clock;
        private readonly ISignals _signals;


        public SitemapController(
            ISitemapService sitemapService,           
            ICacheManager cacheManager,
            IClock clock, 
            ISignals signals) {
            
            _sitemapService = sitemapService;
            _cacheManager = cacheManager;
            _clock = clock;
            _signals = signals;
         
        }


        //GET /sitemap.xml
        public ActionResult Index()
        {
            var doc = _cacheManager.Get<string, XDocument>("sitemap.xml", ctx => {

                ctx.Monitor(_clock.When(TimeSpan.FromHours(1.0)));
                ctx.Monitor(_signals.When("Digic.Sitemap.XmlRefresh"));
               
                return _sitemapService.GetSitemap();
            
            });

            return new XmlResult(doc);
            
        }
      
    }
}