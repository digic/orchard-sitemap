﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digic.Sitemap.Models
{
    public class SitemapCustomRouteRecord
    {
        public virtual int Id { get; set; }
        public virtual string Url { get; set; }
        public virtual bool IndexForDisplay { get; set; }
        public virtual bool IndexForXml { get; set; }
        public virtual string UpdateFrequency { get; set; }
        public virtual int Priority { get; set; }
    }
}