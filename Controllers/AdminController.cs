using Digic.Sitemap.Services;
using Digic.Sitemap.ViewModels;
using Orchard;
using Orchard.Caching;
using Orchard.Core.Contents;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Digic.Sitemap.Controllers
{
    [Admin]
    [ValidateInput(false)]
    public class AdminController : Controller
    {

        private readonly ISitemapService _sitemapService;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;
        private readonly ISignals _signals;

        public dynamic Shape { get; set; }

        public Localizer T { get; set; }

        public AdminController(
            ISitemapService sitemapService,
            IShapeFactory shapeFactory,
            INotifier notifier,
            IOrchardServices services,
            ISignals signals)
        {
            _sitemapService = sitemapService;
            _notifier = notifier;
            _services = services;
            _signals = signals;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }




        // GET: Admin
        public ActionResult Indexing()
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageSitemap, T("Not allowed to manage sitemap")))
                return new HttpUnauthorizedResult();

            var typeSettings = _sitemapService.GetIndexSettings();
            var customRoutes = _sitemapService.GetCustomRoutes();

            var model = new IndexingViewModel
            {
                ContentTypeSettings = typeSettings.OrderBy(q => q.DisplayName).ToList(),
                CustomRoutes = customRoutes.ToList()
            };

            return View(model);
        }


        //POST: Admin
        [HttpPost]
        public ActionResult Indexing(IndexingViewModel model)
        {
            if (!_services.Authorizer.Authorize(Permissions.ManageSitemap, T("Not allowed to manage sitemap")))
                return new HttpUnauthorizedResult();

            if (model.CustomRoutes == null)
            {
                model.CustomRoutes = new List<CustomRouteModel>();
            }

            _sitemapService.SetIndexSettings(model.ContentTypeSettings);
            _sitemapService.SetCustomRoutes(model.CustomRoutes);

            _services.Notifier.Add(NotifyType.Information, T("Saved Sitemap Indexing Settings"));
            _signals.Trigger("Digic.Sitemap.Refresh");
            return RedirectToAction("Indexing");
        }


        public ActionResult GetNewCustomRouteForm()
        {
            var emptyModel = new CustomRouteModel
            {
                IndexForDisplay = false,
                IndexForXml = false,
                Name = string.Empty,
                Priority = 3,
                UpdateFrequency = "weekly",
                Url = string.Empty
            };
            return PartialView("PartialCustomRouteEditor", emptyModel);
        }

    }
}