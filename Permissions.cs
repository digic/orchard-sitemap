using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digic.Sitemap
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageSitemap = new Permission { Description = "Manage Sitemap", Name = "ManageSitemap" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageSitemap,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageSitemap}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {ManageSitemap}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    Permissions = new[] {ManageSitemap}
                },
                new PermissionStereotype {
                    Name = "Author",
                     Permissions = new[] {ManageSitemap}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                     Permissions = new[] {ManageSitemap}
                },
            };
        }

    }
}