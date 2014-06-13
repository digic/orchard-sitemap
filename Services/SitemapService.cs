using Digic.Sitemap.Models;
using Digic.Sitemap.Providers;
using Digic.Sitemap.ViewModels;
using JetBrains.Annotations;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Data;
using Orchard.Services;
using Orchard.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Digic.Sitemap.Extensions;

namespace Digic.Sitemap.Services
{
    public interface ISitemapService : IDependency
    {
        IEnumerable<IndexSettingsModel> GetIndexSettings();
        IEnumerable<CustomRouteModel> GetCustomRoutes();
        
        XDocument GetSitemap();

        void SetIndexSettings(IEnumerable<IndexSettingsModel> settings);
        void SetCustomRoutes(IEnumerable<CustomRouteModel> routes);
    }

    [UsedImplicitly]
    public class SitemapService : ISitemapService
    {
        const string AUTOROUTEPART = "AutoroutePart";
        
        private readonly IRepository<SitemapSettingsRecord> _settingsRepository;
        private readonly IRepository<SitemapCustomRouteRecord> _customRouteRepository; 
        private readonly ISiteService _siteService;
        private readonly IContentManager _contentManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly IEnumerable<ISitemapRouteProvider> _routeProviders;        

        public SitemapService(IRepository<SitemapSettingsRecord> settingsRepository,
            IRepository<SitemapCustomRouteRecord> customRouteRepository,
            ISiteService siteService, 
            ICacheManager cacheManager,
            IContentManager contentManager,                  
            IEnumerable<ISitemapRouteProvider> routeProviders,          
            ISignals signals) {
          
            _settingsRepository = settingsRepository;
            _customRouteRepository = customRouteRepository;
            _siteService = siteService;
            _cacheManager = cacheManager;
            _contentManager = contentManager;          
            _routeProviders = routeProviders;
            _signals = signals;

            }


        public IEnumerable<IndexSettingsModel> GetIndexSettings()
        {
            var settings = _cacheManager.Get<string, IEnumerable<IndexSettingsModel>>("Digic.Sitemap.IndexSettings", 
                ctx =>
                {
                    
                    var contentTypes = _contentManager.GetContentTypeDefinitions()
                        .Where(ctd => ctd.Parts.FirstOrDefault(p => p.PartDefinition.Name == "AutoroutePart") != null)
                        .ToList();

                    var typeNames = contentTypes.Select(t => t.Name).ToArray();

                    // Delete everything that no longer corresponds to these allowed content types
                    var toDelete = _settingsRepository.Fetch(q => !typeNames.Contains(q.ContentType)).ToList();
                    foreach (var record in toDelete)
                    {
                        _settingsRepository.Delete(record);
                    }
                    _settingsRepository.Flush();



                    var contentSettings = new List<IndexSettingsModel>();
                    foreach (var type in contentTypes)
                    {
                        var _type = type;
                        // Get the record, generate a new one if it doesn't exist.
                        SitemapSettingsRecord record = _settingsRepository.Fetch(q => q.ContentType == _type.Name).FirstOrDefault();
                        if (record == null)
                        {
                            record = new SitemapSettingsRecord();
                            record.ContentType = type.Name;
                            record.IndexForDisplay = false;
                            record.IndexForXml = false;
                            record.Priority = 3;
                            record.UpdateFrequency = "weekly";
                            _settingsRepository.Create(record);
                        }

                        var model = new IndexSettingsModel()
                        {
                            DisplayName = type.DisplayName,
                            Name = type.Name,
                            IndexForDisplay = record.IndexForDisplay,
                            IndexForXml = record.IndexForXml,
                            Priority = record.Priority,
                            UpdateFrequency = record.UpdateFrequency
                        };
                        contentSettings.Add(model);
                    }


                    ctx.Monitor(_signals.When("ContentDefinitionManager"));
                    ctx.Monitor(_signals.When("Digic.Sitemap.IndexSettings"));

                    return contentSettings;
                });

            return settings;
        }

        public void SetIndexSettings(IEnumerable<IndexSettingsModel> settings)
        {
            foreach (var item in settings)
            {
                var record = _settingsRepository.Get(q => q.ContentType == item.Name);
                if (record == null)
                {
                    record = new SitemapSettingsRecord();
                    record.ContentType = item.Name;
                    record.IndexForDisplay = item.IndexForDisplay;
                    record.IndexForXml = item.IndexForXml;
                    record.Priority = item.Priority;
                    record.UpdateFrequency = item.UpdateFrequency;
                    _settingsRepository.Create(record);
                }
                else
                {
                    record.IndexForDisplay = item.IndexForDisplay;
                    record.IndexForXml = item.IndexForXml;
                    record.Priority = item.Priority;
                    record.UpdateFrequency = item.UpdateFrequency;
                    _settingsRepository.Update(record);
                }
            }
            _signals.Trigger("Digic.Sitemap.IndexSettings");

        }

        public IEnumerable<CustomRouteModel> GetCustomRoutes()
        {
            return _cacheManager.Get("Digic.Sitemap.CustomRoutes", 
            ctx =>
            {
                ctx.Monitor(_signals.When("Digic.Sitemap.CustomRoutes"));

                var records = _customRouteRepository.Table.ToList();
                return records.Select(r => new CustomRouteModel
                {
                    IndexForDisplay = r.IndexForDisplay,
                    IndexForXml = r.IndexForXml,
                    Name = r.Url.Trim('/').Split('/')[0].SlugToTitle(),
                    Priority = r.Priority,
                    UpdateFrequency = r.UpdateFrequency ?? "weekly",
                    Url = r.Url ?? String.Empty
                });
            });
        }

        public void SetCustomRoutes(IEnumerable<CustomRouteModel> routes)
        {
            var existingRouteIds = new List<int>();

            foreach (var model in routes)
            {
                // Treat empty url as over-ride for root path
                if (string.IsNullOrWhiteSpace(model.Url))
                {
                    model.Url = string.Empty;
                }

                var customRouteModel = model;
                var record = _customRouteRepository.Get(q => q.Url == model.Url);
                if (record == null)
                {
                    record = new SitemapCustomRouteRecord
                    {
                        IndexForDisplay = model.IndexForDisplay,
                        IndexForXml = model.IndexForXml,
                        Priority = model.Priority,
                        Url = model.Url ?? String.Empty,                       
                        UpdateFrequency = model.UpdateFrequency
                    };
                    _customRouteRepository.Create(record);
                }
                else
                {
                    record.IndexForDisplay = model.IndexForDisplay;
                    record.IndexForXml = model.IndexForXml;
                    record.Priority = model.Priority;
                    record.UpdateFrequency = model.UpdateFrequency;
                    _customRouteRepository.Update(record);
                }
                existingRouteIds.Add(record.Id);
            }
            _customRouteRepository.Flush();

            var toDelete = _customRouteRepository.Fetch(q => !existingRouteIds.Contains(q.Id));
            foreach (var record in toDelete)
            {
                _customRouteRepository.Delete(record);
            }
            _customRouteRepository.Flush();

            _signals.Trigger("Digic.Sitemap.CustomRoutes");
        }

        public void DeleteCustomRoute(string url)
        {
            var record = _customRouteRepository.Get(q => q.Url == url);
            if (record != null)
            {
                _customRouteRepository.Delete(record);
                _customRouteRepository.Flush();
                _signals.Trigger("Digic.Sitemap.CustomRoutes");
            }
        }



        public XDocument GetSitemap() 
        {
            var rootUrl = GetRootPath();
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            
                var root = new XElement(xmlns + "urlset");
                document.Add(root);


                foreach (var provider in _routeProviders.OrderByDescending( p => p.Priority)) 
                {
                    root.Add(provider.GetRoutes()
                        //.Where(r => _routeFilters.All(filter => filter.AllowUrl(r.Url)))
                        .Select(route => GetSitemapRoute(route))
                        );
                }
            
                return document;
        }

        public XElement GetSitemapRoute(SitemapRoute route) 
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var rootUrl = GetRootPath();
            string url = route.Url;
            if (!Regex.IsMatch(route.Url, @"^\w+://.*$"))
            {
                url = rootUrl + route.Url.TrimStart('/');
            }


            var element = new XElement(xmlns + "url");
            element.Add(new XElement(xmlns + "loc", url));
            element.Add(new XElement(xmlns + "changefreq", route.UpdateFrequency));
            if (route.LastUpdated.HasValue)
            {
                element.Add(new XElement(xmlns + "lastmod", route.LastUpdated.Value.ToString("yyyy-MM-dd")));
            }
            var priority = (route.Priority - 1) / 4.0;
            if (priority >= 0.0 && priority <= 1.0)
            {
                element.Add(new XElement(xmlns + "priority", (route.Priority - 1) / 4.0));
            }


            return element;
        }

        private string GetRootPath()
        {
            var baseUrl = _siteService.GetSiteSettings().BaseUrl;
            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";
            return baseUrl;
        }




        //public IEnumerable<IndexSettingsModel> GetContentIndexSettings() 
        //{
        //    var types = GetAvailableContentTypes().Select(def => new IndexSettingsModel() 
        //    {
        //        DisplayName = def.DisplayName,
        //        Name = def.Name                
        //    });

        //    var indexedTypes = GetContentTypeSettings();



        //}

        

        //public IEnumerable<IndexSettingsModel> GetContentTypeSettings()
        //{
        //    return _settingsRepository.Table.Select( record => new IndexSettingsModel() {

        //        DisplayName = record.ContentType,
        //        Name = record.ContentType,
        //        IndexForDisplay = record.IndexForDisplay,
        //        IndexForXml = record.IndexForXml,
        //        Priority = record.Priority,
        //        UpdateFrequency = record.UpdateFrequency
                
        //    });

        //}

        //public void SetIndexSettings(IEnumerable<IndexSettingsModel> settings)
        //{
        //    foreach (var item in settings)
        //    {              
        //        var record = _settingsRepository.Get(q => q.ContentType == item.Name);
        //        if (record == null)
        //        {
        //            record = new SitemapSettingsRecord();
        //            record.ContentType = item.Name;
        //            record.IndexForDisplay = item.IndexForDisplay;
        //            record.IndexForXml = item.IndexForXml;
        //            record.Priority = item.Priority;
        //            record.UpdateFrequency = item.UpdateFrequency;
        //            _settingsRepository.Create(record);
        //        }
        //        else
        //        {
        //            record.IndexForDisplay = item.IndexForDisplay;
        //            record.IndexForXml = item.IndexForXml;
        //            record.Priority = item.Priority;
        //            record.UpdateFrequency = item.UpdateFrequency;
        //            _settingsRepository.Update(record);
        //        }
        //    }
        //    _signals.Trigger("Digic.Sitemap.IndexSettings");
            
        //}

        //private IEnumerable<ContentTypeDefinition> GetAvailableContentTypes() {

        //    return _contentManager.GetContentTypeDefinitions()
        //            .Where(ctd => ctd.Parts.FirstOrDefault(p => p.PartDefinition.Name == AUTOROUTEPART) != null);
                      
        //}

    }
}