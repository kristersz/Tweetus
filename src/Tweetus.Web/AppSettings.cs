using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web
{
    public class AppSettings
    {
        public string SiteTitle { get; set; }
        public string MongoDbConnectionString { get; set; }
        public string MongoDbName { get; set; }
    }
}
