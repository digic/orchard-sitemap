using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digic.Sitemap
{
    public class AdminMenu : INavigationProvider
    {

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Sitemap"), "9", 
                menu => menu.Permission(Permissions.ManageSitemap)
                    .Add(T("Indexing"), "1", item => item.Action("Indexing", "Admin", new { area = "Digic.Sitemap" }).LocalNav().Permission(Permissions.ManageSitemap))
                    .Add(T("Cache"), "3", item => item.Action("Cache", "Admin", new { area = "Digic.Sitemap" }).LocalNav().Permission(Permissions.ManageSitemap)));            
        }
    }
}