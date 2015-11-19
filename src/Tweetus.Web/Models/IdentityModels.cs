using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Identity3.MongoDB;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Framework.OptionsModel;
using MongoDB.Driver;

namespace Tweetus.Web.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public byte[] ProfilePicture { get; set; }

        public DateTime JoinedOn { get; set; }
    }

    public class ApplicationDbContext : MongoIdentityContext<ApplicationUser, IdentityRole>
    {
        public ApplicationDbContext(IOptions<AppSettings> appSettings)
            : base()
        {
            var client = new MongoClient(appSettings.Value.MongoDbConnectionString);
            var database = client.GetDatabase(appSettings.Value.MongoDbName);

            this.Users = database.GetCollection<ApplicationUser>("users");
            this.Roles = database.GetCollection<IdentityRole>("roles");
        }
    }
}
