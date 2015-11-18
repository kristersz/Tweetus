using System;
using System.Collections.Generic;
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
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.Services;
using Tweetus.Web.Services.Mappers;
using Tweetus.Web.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Tweetus.Web.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly TweetManager _tweetManager;
        private readonly UserRelationshipManager _userRelationshipManager;

        public UserController(UserManager<ApplicationUser> userManager, TweetManager tweetManager, UserRelationshipManager userRelationshipManager) : base(userManager)
        {
            _tweetManager = tweetManager;
            _userRelationshipManager = userRelationshipManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardVM();
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            var userObjectId = new ObjectId(User.GetUserId());

            var followingUserIds = await _userRelationshipManager.GetFollowingUsers(userObjectId);

            if (followingUserIds.Count > 0)
            {
                var allTweets = await _tweetManager.GetTweetsByUserIds(followingUserIds);

                if (allTweets.Count() > 0)
                {
                    foreach (var t in allTweets)
                    {
                        var tweetUser = await _userManager.FindByIdAsync(t.UserId.ToString());
                        viewModel.Tweets.Add(TweetMapper.MapTweetToViewModel(t, tweetUser));
                    }
                }
            }

            viewModel.ProfilePicture = Convert.ToBase64String(user.ProfilePicture);
            viewModel.TweetCount = await _tweetManager.GetUserTweetCount(userObjectId);
            viewModel.FollowerCount = await _userRelationshipManager.GetUserFollowerCount(userObjectId);
            viewModel.FollowingCount = await _userRelationshipManager.GetUserFollowingCount(userObjectId);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string username)
        {
            var viewModel = new ProfileVM();

            var normalizedUsername = username.ToUpper();
            var user = await _userManager.FindByNameAsync(normalizedUsername);
            var userObjectId = new ObjectId(user.Id);

            viewModel.UserId = user.Id;
            viewModel.UserName = user.UserName;
            viewModel.FullName = user.UserName;

            if (user.ProfilePicture != null)
            {
                viewModel.ProfilePicture = Convert.ToBase64String(user.ProfilePicture);
            }
            

            viewModel.IsViewersProfile = User.GetUserId() == user.Id;
            viewModel.ViewerAlreadyFollowing = await _userRelationshipManager.IsUserFollowing(new ObjectId(User.GetUserId()), userObjectId);

            var allTweets = await _tweetManager.GetTweetsByUserId(userObjectId);

            if (allTweets.Count() > 0)
                viewModel.Tweets.AddRange(TweetMapper.MapTweetsToViewModels(allTweets, user));

            var followingUserIds = await _userRelationshipManager.GetFollowingUsers(userObjectId);

            if (followingUserIds.Count > 0)
            {
                foreach (var followingUserId in followingUserIds)
                {
                    var followingUser = await _userManager.FindByIdAsync(followingUserId.ToString());

                    viewModel.FollowingUsers.Add(new FollowingUserVM()
                    {
                        FollowingUserId = followingUser.Id,
                        FollowingUserName = followingUser.UserName,
                        FollowingUserAbout = "BLANK"
                    });
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> FollowUser(string userId)
        {
            var result = new JsonServiceResult<bool>();

            try
            {
                await _userRelationshipManager.AddFollowingUser(new ObjectId(User.GetUserId()), new ObjectId(userId));

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
