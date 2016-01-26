using System;
using AspNet.Identity3.MongoDB;
using Microsoft.Extensions.OptionsModel;
using MongoDB.Driver;

namespace Tweetus.Web.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string About { get; set; }
        public string Website { get; set; }
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
