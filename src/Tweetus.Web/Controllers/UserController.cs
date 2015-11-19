using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity3.MongoDB;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using MongoDB.Bson;
using Tweetus.Web.Data;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.Data.Enums;
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.Services;
using Tweetus.Web.Services.Mappers;
using Tweetus.Web.Utilities.Extensions;
using Tweetus.Web.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Tweetus.Web.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly TweetManager _tweetManager;
        private readonly UserRelationshipManager _userRelationshipManager;
        private readonly NotificationManager _notificationManager;

        public UserController(UserManager<ApplicationUser> userManager, 
            TweetManager tweetManager, 
            UserRelationshipManager userRelationshipManager,
            NotificationManager notificationManager) : base(userManager)
        {
            _tweetManager = tweetManager;
            _userRelationshipManager = userRelationshipManager;
            _notificationManager = notificationManager;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardVM();

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
                    viewModel.Tweets.Add(TweetMapper.MapTweetToViewModel(t, tweetUser));
                }
            }

            viewModel.FullName = user.FullName;
            viewModel.TweetCount = await _tweetManager.GetUserTweetCount(userObjectId);
            viewModel.FollowerCount = await _userRelationshipManager.GetUserFollowerCount(userObjectId);
            viewModel.FollowingCount = await _userRelationshipManager.GetUserFollowingCount(userObjectId);

            if (user.ProfilePicture != null)
            {
                viewModel.ProfilePicture = Convert.ToBase64String(user.ProfilePicture);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string username)
        {
            var viewModel = new ProfileVM();

            var normalizedUsername = username.ToUpper();
            var user = await _userManager.FindByNameAsync(normalizedUsername);
            var userObjectId = user.Id.ToObjectId();

            viewModel.UserId = user.Id;
            viewModel.UserName = user.UserName;
            viewModel.FullName = user.FullName;
            viewModel.JoinedOn = user.JoinedOn;

            if (user.ProfilePicture != null)
            {
                viewModel.ProfilePicture = Convert.ToBase64String(user.ProfilePicture);
            }
            

            viewModel.IsViewersProfile = User.GetUserId() == user.Id;
            viewModel.ViewerAlreadyFollowing = await _userRelationshipManager.IsUserFollowing(User.GetUserId().ToObjectId(), userObjectId);

            var allTweets = await _tweetManager.GetTweetsByUserId(userObjectId);

            if (allTweets.Count() > 0)
                viewModel.Tweets.AddRange(TweetMapper.MapTweetsToViewModels(allTweets, user));

            var followingUserIds = await _userRelationshipManager.GetFollowingUsers(userObjectId);

            if (followingUserIds.Count > 0)
            {
                foreach (var followingUserId in followingUserIds)
                {
                    var followingUser = await _userManager.FindByIdAsync(followingUserId.ToString());

                    viewModel.FollowingUsers.Add(new UserVM()
                    {
                        UserId = followingUser.Id,
                        UserName = followingUser.UserName,
                        FullName = followingUser.FullName,
                        UserAbout = "BLANK",
                        ProfilePictureBase64 = (followingUser.ProfilePicture != null) ? Convert.ToBase64String(followingUser.ProfilePicture) : string.Empty
                    });
                }
            }

            var followerUserIds = await _userRelationshipManager.GetUserFollowers(userObjectId);

            if (followerUserIds.Count > 0)
            {
                foreach (var followerUserId in followerUserIds)
                {
                    var follower = await _userManager.FindByIdAsync(followerUserId.ToString());

                    viewModel.Followers.Add(new UserVM()
                    {
                        UserId = follower.Id,
                        UserName = follower.UserName,
                        FullName = follower.FullName,
                        UserAbout = "BLANK",
                        ProfilePictureBase64 = (follower.ProfilePicture != null) ? Convert.ToBase64String(follower.ProfilePicture) : string.Empty
                    });
                }
            }

            var likedTweets = await _tweetManager.GetUserLikedTweets(userObjectId);

            if (likedTweets.Count() > 0)
                viewModel.LikedTweets.AddRange(TweetMapper.MapTweetsToViewModels(likedTweets, user));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> FollowUser(string userId)
        {
            var result = new JsonServiceResult<bool>();

            try
            {
                var currentUserId = User.GetUserId();

                await _userRelationshipManager.AddFollowingUser(currentUserId.ToObjectId(), userId.ToObjectId());
                await _notificationManager.CreateNewNotification(userId.ToObjectId(), NotificationType.Follow, currentUserId.ToObjectId(), null);

                result.IsValid = true;
                result.Value = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }
    }
}
