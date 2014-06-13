using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digic.Sitemap
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("AdminDisplay").SetUrl("admin.displaysettings.js").SetDependencies("jQueryUI");
            manifest.DefineScript("AdminIndex").SetUrl("admin.indexsettings.js").SetDependencies("jQuery");

        }
    }
}