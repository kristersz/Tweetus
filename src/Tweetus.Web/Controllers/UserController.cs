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

                var currentUserLikedTweets = await _tweetManager.GetUserLikedTweets(userObjectId);

                foreach (var tweet in viewModel.Tweets)
                {
                    tweet.CanRetweet = tweet.TweetedByUserId != user.Id;
                    tweet.CurrentUserAlreadyLiked = currentUserLikedTweets.Any(t => t.Id == tweet.TweetId.ToObjectId());
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
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Error", "Home"); 

            var profileOwner = await _userManager.FindByNameAsync(username.ToUpper());

            if (profileOwner == null)
                return RedirectToAction("Error", "Home");

            var viewModel = new ProfileVM();

            var currentUserId = User.GetUserId();
            var profileOwnerObjectId = profileOwner.Id.ToObjectId();

            // map user fields
            viewModel.UserId = profileOwner.Id;
            viewModel.UserName = profileOwner.UserName;
            viewModel.FullName = profileOwner.FullName;
            viewModel.JoinedOn = profileOwner.JoinedOn;
            viewModel.About = profileOwner.About;
            viewModel.Website = profileOwner.Website;

            if (profileOwner.ProfilePicture != null)
                viewModel.ProfilePicture = Convert.ToBase64String(profileOwner.ProfilePicture);         

            viewModel.IsCurrentUsersProfile = currentUserId == profileOwner.Id;
            viewModel.CurrentUserAlreadyFollowing = await _userRelationshipManager.IsUserFollowing(User.GetUserId().ToObjectId(), profileOwnerObjectId);

            // get users tweets
            var allTweets = await _tweetManager.GetTweetsByUserId(profileOwnerObjectId);

            if (allTweets.Count() > 0)
            {
                viewModel.Tweets.AddRange(TweetMapper.MapTweetsToViewModels(allTweets, profileOwner));

                var currentUserLikedTweets = await _tweetManager.GetUserLikedTweets(currentUserId.ToObjectId());

                foreach (var tweet in viewModel.Tweets)
                {
                    tweet.CanRetweet = !viewModel.IsCurrentUsersProfile;
                    tweet.CurrentUserAlreadyLiked = currentUserLikedTweets.Any(t => t.Id == tweet.TweetId.ToObjectId());
                }
            }

            // get the people that this user is following
            var followingUserIds = await _userRelationshipManager.GetFollowingUsers(profileOwnerObjectId);

            if (followingUserIds.Count > 0)
            {
                foreach (var followingUserId in followingUserIds)
                {
                    var followingUser = await _userManager.FindByIdAsync(followingUserId.ToString());

                    viewModel.FollowingUsers.Add(UserMapper.MapUserToViewModel(followingUser));
                }
            }

            // get users followers
            var followerUserIds = await _userRelationshipManager.GetUserFollowers(profileOwnerObjectId);

            if (followerUserIds.Count > 0)
            {
                foreach (var followerUserId in followerUserIds)
                {
                    var follower = await _userManager.FindByIdAsync(followerUserId.ToString());

                    viewModel.Followers.Add(UserMapper.MapUserToViewModel(follower));
                }
            }

            // get the tweets that this user liked
            var likedTweets = await _tweetManager.GetUserLikedTweets(profileOwnerObjectId);

            if (likedTweets.Count() > 0)
            {
                foreach (var likedTweet in likedTweets)
                {
                    var likedTweetUser = await _userManager.FindByIdAsync(likedTweet.UserId.ToString());

                    viewModel.LikedTweets.Add(TweetMapper.MapTweetToViewModel(likedTweet, likedTweetUser));
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SearchUsers(string searchParam)
        {
            var result = new JsonServiceResult<List<UserVM>>();

            if (string.IsNullOrEmpty(searchParam))
                return Json(result);

            try
            {
                var user = await _userManager.FindByNameAsync(searchParam.ToUpper());

                if (user != null)
                {
                    var vm = UserMapper.MapUserToViewModel(user);

                    result.Value = new List<UserVM>() { vm };
                    result.IsValid = true;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
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
