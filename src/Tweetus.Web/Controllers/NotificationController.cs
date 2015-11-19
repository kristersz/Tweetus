using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Tweetus.Web.Data.Enums;
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.Utilities.Extensions;
using Tweetus.Web.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Tweetus.Web.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly TweetManager _tweetManager;
        private readonly NotificationManager _notificationManager;

        public NotificationController(UserManager<ApplicationUser> userManager,
            TweetManager tweetManager,
            NotificationManager notificationManager) : base(userManager)
        {
            _tweetManager = tweetManager;
            _notificationManager = notificationManager;
        }

        // GET: /<controller>/
        public async Task<IActionResult> List()
        {
            var viewModels = new List<NotificationVM>();

            var userNotifications = await _notificationManager.GetUserNotifications(User.GetUserId().ToObjectId());

            if (userNotifications != null)
            {
                foreach (var notification in userNotifications.Notifications)
                {
                    var vm = new NotificationVM();
                    var user = await _userManager.FindByIdAsync(notification.UserId.ToString());

                    vm.Type = (NotificationType)notification.Type;
                    vm.UserId = user.Id;
                    vm.UserName = user.UserName;
                    vm.CreatedOn = notification.CreatedOn;

                    if (vm.Type == NotificationType.Like)
                    {
                        var tweet = await _tweetManager.GetTweetById(notification.TweetId);

                        vm.TweetContent = tweet.Content;
                        vm.TweetedOn = tweet.CreatedOn;
                    }

                    viewModels.Add(vm);
                }
            }

            viewModels = viewModels.OrderByDescending(v => v.CreatedOn).ToList();           

            return View(viewModels);
        }
    }
}
