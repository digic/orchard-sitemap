using Digic.Sitemap.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Digic.Sitemap
{
    public class Migrations : DataMigrationImpl {
        
        public int Create() {

            SchemaBuilder.CreateTable("SitemapSettingsRecord", table => table
                   .Column<int>("Id", col => col.PrimaryKey().Identity())
                   .Column<string>("ContentType", col => col.Unique())
                   .Column<bool>("IndexForDisplay", col => col.WithDefault(true))
                   .Column<bool>("IndexForXml", col => col.WithDefault(true))
                   .Column<string>("UpdateFrequency")
                   .Column<int>("Priority")
               );

            return 1;
        }

        public int UpdateFrom1() 
        {
            SchemaBuilder.CreateTable("SitemapCustomRouteRecord", table => table
               .Column<int>("Id", col => col.PrimaryKey().Identity())
               .Column<string>("Url", col => col.Unique())
               .Column<bool>("IndexForDisplay", col => col.WithDefault(true))
               .Column<bool>("IndexForXml", col => col.WithDefault(true))
               .Column<string>("UpdateFrequency")
               .Column<int>("Priority")
           );

            return 2;
        }

     
    }
}