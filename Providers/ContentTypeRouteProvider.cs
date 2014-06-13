using Digic.Sitemap.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Autoroute;
using Orchard.Autoroute.Models;

namespace Digic.Sitemap.Providers
{
    public class ContentTypeRouteProvider : ISitemapRouteProvider
    {
        private readonly IRepository<SitemapSettingsRecord> _settings;
        private readonly IContentManager _contentManager;


        public ContentTypeRouteProvider(IRepository<SitemapSettingsRecord> settings, IContentManager contentManager) {

            _settings = settings;
            _contentManager = contentManager;
            
        }

        public IEnumerable<SitemapRoute> GetRoutes(){
              // Get all active content types
            var types = _settings.Fetch(q => q.IndexForXml)
                .ToDictionary(
                    k => k.ContentType,
                    v => v);

            if (types.Any()) {
                var contents = _contentManager.Query(VersionOptions.Published, types.Keys.ToArray()).List();

                return contents.Select(c => new SitemapRoute {
                    Priority = types[c.ContentType].Priority,
                    Title = _contentManager.GetItemMetadata(c).DisplayText,
                    UpdateFrequency = types[c.ContentType].UpdateFrequency,
                    Url = c.As<AutoroutePart>().Path,
                    LastUpdated = c.Has<CommonPart>() ? c.As<CommonPart>().ModifiedUtc : null
                });
            }

            return new List<SitemapRoute>();
        }

        public int Priority
        {
            get { return 1; }
        }
    }
}
