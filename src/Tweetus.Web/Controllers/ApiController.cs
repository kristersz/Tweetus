using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Tweetus.Web.Helpers;
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.Models.Integration;
using Tweetus.Web.Utilities.Extensions;

namespace Tweetus.Web.Controllers
{
    [Route("api/tweets")]
    public class ApiController : BaseController
    {
        private readonly TweetManager _tweetManager;

        public ApiController(UserManager<ApplicationUser> userManager, TweetManager tweetManager) : base(userManager)
        {
            _tweetManager = tweetManager;
        }

        // GET: api/tweets
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var tweets = await _tweetManager.GetAllTweets();
            var exportableTweets = new List<ExportableTweet>();

            foreach (var tweet in tweets)
            {
                var tweetUser = await _userManager.FindByIdAsync(tweet.UserId.ToString());

                exportableTweets.Add(new ExportableTweet()
                {
                    TweetId = tweet.Id.ToString(),
                    UserName = tweetUser.UserName,
                    Text = tweet.Content,
                    Latitude = tweet.Latitude,
                    Longitude = tweet.Longitude,
                    ImageUrl = string.IsNullOrEmpty(tweet.FilePath) ? string.Empty : string.Format("http://{0}{1}", HttpContext.Request.Host, tweet.FilePath),
                    CreatedOn = tweet.CreatedOn
                });
            }

            return Json(exportableTweets);
        }

        // GET api/tweets/1/1/1
        [HttpGet("{latitude}/{longitude}/{radius}")]
        public async Task<JsonResult> Get(double latitude, double longitude, double radius)
        {
            var latLng = new LatLng(latitude, longitude);

            var tweets = await _tweetManager.GetAllTweets();
            var exportableTweets = new List<ExportableTweet>();

            foreach (var tweet in tweets)
            {
                if (tweet.Latitude > 0 && tweet.Longitude > 0)
                {
                    var tweetLatLng = new LatLng(tweet.Latitude, tweet.Longitude);

                    if (LatLngHelper.IsWithinRadius(latLng, tweetLatLng, radius))
                    {
                        var tweetUser = await _userManager.FindByIdAsync(tweet.UserId.ToString());

                        exportableTweets.Add(new ExportableTweet()
                        {
                            TweetId = tweet.Id.ToString(),
                            UserName = tweetUser.UserName,
                            Text = tweet.Content,
                            Latitude = tweet.Latitude,
                            Longitude = tweet.Longitude,
                            ImageUrl = string.IsNullOrEmpty(tweet.FilePath) ? string.Empty : string.Format("http://{0}{1}", HttpContext.Request.Host, tweet.FilePath),
                            CreatedOn = tweet.CreatedOn
                        });
                    }
                }               
            }

            return Json(exportableTweets);
        }
    }
}
