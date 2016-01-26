using AspNet.Identity3.MongoDB;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Framework.DependencyInjection;
using Tweetus.Web.Data;
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.Services;

namespace Tweetus.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ApplicationDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddMongoStores<ApplicationDbContext, ApplicationUser, IdentityRole>();

            services.AddMvc();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
          
            services.AddSingleton<IMongoDbRepository, MongoDbRepository>();
            services.AddTransient<TweetManager, TweetManager>();
            services.AddTransient<UserRelationshipManager, UserRelationshipManager>();
            services.AddTransient<ConversationManager, ConversationManager>();
            services.AddTransient<NotificationManager, NotificationManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(options =>
                {
                    options.EnableAll();
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();
            app.UseStaticFiles();
            app.UseIdentity();

            app.UseFacebookAuthentication(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "profile",
                    template: "User/{username}",
                    defaults: new { controller = "User", action = "Profile" });
            });
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
