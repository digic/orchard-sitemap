using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Digic.Sitemap
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                            "Admin/Sitemap",
                         new RouteValueDictionary {
                            {"area", "Digic.Sitemap"},
                            {"controller", "Admin"},
                            {"action", "Indexing"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Digic.Sitemap"}
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Route = new Route(
                        "Admin/Sitemap/GetCustomRouteForm",
                        new RouteValueDictionary {
                            {"area", "Digic.Sitemap"},
                            {"controller", "Admin"},
                            {"action", "GetNewCustomRouteForm"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Digic.Sitemap"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "sitemap.xml",
                        new RouteValueDictionary {
                            {"area", "Digic.Sitemap"},
                            {"controller", "Sitemap"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Digic.Sitemap"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}