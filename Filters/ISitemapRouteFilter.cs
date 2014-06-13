using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digic.Sitemap.Filters
{
    public interface ISitemapRouteFilter
    {
        /// <summary>
        /// Filter function to disallow certain paths from appearing for display and xml.
        /// </summary>
        /// <param name="path">The relative path of the found content item.</param>
        /// <returns>True if this url should be allowed in the sitemap. False if not.</returns>
        bool AllowUrl(string path);
    }
}
