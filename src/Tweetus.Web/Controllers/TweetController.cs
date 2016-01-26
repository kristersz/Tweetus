using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.Data.Enums;
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.Services.Mappers;
using Tweetus.Web.Utilities;
using Tweetus.Web.Utilities.Extensions;
using Tweetus.Web.ViewModels;

namespace Tweetus.Web.Controllers
{
    public class TweetController : BaseController
    {
        private readonly TweetManager _tweetManager;
        private readonly UserRelationshipManager _userRelationshipManager;
        private readonly NotificationManager _notificationManager;
        private readonly IHostingEnvironment _environment;

        public TweetController(UserManager<ApplicationUser> userManager, 
            TweetManager tweetManager, 
            UserRelationshipManager userRelationshipManager,
            NotificationManager notificationManager,
            IHostingEnvironment environment) : base(userManager)
        {
            _tweetManager = tweetManager;
            _userRelationshipManager = userRelationshipManager;
            _notificationManager = notificationManager;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetTweetsForDashboard(string userId)
        {
            var result = new JsonServiceResult<List<TweetVM>>();

            try
            {
                var tweets = new List<TweetVM>();

                var user = await GetCurrentUserAsync();
                var userObjectId = user.Id.ToObjectId();

                var allUserIds = new List<ObjectId>() { userObjectId };
                var followingUserIds = await _userRelationshipManager.GetFollowingUsers(userObjectId);

                if (followingUserIds.Count > 0)
                    allUserIds.AddRange(followingUserIds);

                var allTweets = await _tweetManager.GetTweetsByUserIds(allUserIds);

                if (allTweets.Count() > 0)
                {
                    foreach (var t in allTweets)
                    {
                        var tweetUser = await _userManager.FindByIdAsync(t.UserId.ToString());
                        tweets.Add(TweetMapper.MapTweetToViewModel(t, tweetUser));
                    }

                    var currentUserLikedTweets = await _tweetManager.GetUserLikedTweets(userObjectId);

                    foreach (var tweet in tweets)
                    {
                        tweet.CanRetweet = tweet.TweetedByUserId != user.Id;
                        tweet.CurrentUserAlreadyLiked = currentUserLikedTweets.Any(t => t.Id == tweet.TweetId.ToObjectId());
                    }
                }

                result.Value = tweets;
                result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }           

            return Json(result);
        }

        [HttpPost]
        [Produces("text/plain")]
        public async Task<JsonResult> PostTweet(PostTweetVM tweet)
        {
            var result = new JsonServiceResult<TweetVM>();

            try
            {
                var user = await GetCurrentUserAsync();
                var now = DateTime.Now;
                var file = tweet.File;

                var newTweet = new Tweet()
                {
                    Content = tweet.Content,
                    UserId = user.Id.ToObjectId(),
                    CreatedOn = now
                };

                if (tweet.LocationEnabled)
                {
                    double latitude;
                    double longitude;

                    if (Double.TryParse(tweet.Latitude, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out latitude)
                        && Double.TryParse(tweet.Longitude, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out longitude))
                    {
                        newTweet.Latitude = latitude;
                        newTweet.Longitude = longitude;
                    }
                }

                if (file != null)
                {
                    var allowedExtensions = new string[] { ".jpg", ".png", ".gif" };

                    if (file.Length > 0)
                    {
                        var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                        var fileName = parsedContentDisposition.FileName.Trim('"');

                        string ext = Path.GetExtension(fileName).ToLower();

                        if (allowedExtensions.Contains(ext))
                        {
                            var userAsciiName = user.NormalizedUserName.ToAsciiString();
                            var fileNameHash = fileName.GetHashCode();

                            if (fileNameHash < 0)
                                fileNameHash = fileNameHash * -1;

                            var mediaDir = Path.Combine(_environment.WebRootPath, "media");
                            var userDir = Path.Combine(mediaDir, userAsciiName);

                            Directory.CreateDirectory(userDir);

                            var filePath = Path.Combine(userDir, fileNameHash.ToString());

                            await file.SaveAsAsync(string.Format("{0}{1}", filePath, ext));

                            newTweet.FileMimeType = file.ContentType;
                            newTweet.FilePath = string.Format("/media/{0}/{1}{2}", userAsciiName, fileNameHash, ext);
                        }
                    }
                }

                await _tweetManager.CreateTweet(newTweet);

                var mentionedUserNames = TweetUtils.ExtractUserMentions(tweet.Content);

                foreach (var mentionedUserName in mentionedUserNames)
                {
                    var mentionedUser = await _userManager.FindByNameAsync(mentionedUserName.ToUpper());

                    if (mentionedUser != null)
                    {
                        await _notificationManager.CreateNewNotification(mentionedUser.Id.ToObjectId(), NotificationType.Mention, user.Id.ToObjectId(), newTweet.Id);
                    }
                }

                var viewModel = new TweetVM();

                viewModel.Content = tweet.Content;
                viewModel.TweetedByFullName = user.FullName;
                viewModel.TweetedByUserName = user.UserName;
                viewModel.TweetedOn = now;

                result.Value = viewModel;
                result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result); 
        }

        [HttpPost]
        public async Task<JsonResult> LikeTweet(string tweetId)
        {
            var result = new JsonServiceResult<bool>();

            try
            {
                var currentUserObjectId = User.GetUserId().ToObjectId();
                var tweetObjectId = tweetId.ToObjectId();

                var tweet = await _tweetManager.GetTweetById(tweetObjectId);

                await _tweetManager.LikeTweet(currentUserObjectId, tweetObjectId);
                await _notificationManager.CreateNewNotification(tweet.UserId, NotificationType.Like, currentUserObjectId, tweetObjectId);

                result.Value = result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> Retweet(string tweetId)
        {
            var result = new JsonServiceResult<bool>();

            try
            {
                var currentUserObjectId = User.GetUserId().ToObjectId();
                var tweetObjectId = tweetId.ToObjectId();

                var originalTweet = await _tweetManager.GetTweetById(tweetObjectId);
                var originalTweetUser = await _userManager.FindByIdAsync(originalTweet.UserId.ToString());

                await _tweetManager.CreateTweet(new Tweet() {
                    Content = originalTweet.Content,
                    UserId = currentUserObjectId,
                    FileMimeType = originalTweet.FileMimeType,
                    FilePath = originalTweet.FilePath,
                    CreatedOn = DateTime.Now,
                    RetweetedFromId = originalTweet.Id,
                    RetweetedFromUserName = originalTweetUser.UserName
                });

                await _notificationManager.CreateNewNotification(originalTweet.UserId, NotificationType.Retweet, currentUserObjectId, tweetObjectId);

                result.Value = result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }
    }
}
