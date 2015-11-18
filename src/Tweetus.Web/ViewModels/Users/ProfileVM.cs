using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.ViewModels
{
    public class ProfileVM
    {
        public ProfileVM()
        {
            Tweets = new List<TweetVM>();
            FollowingUsers = new List<FollowingUserVM>();
        }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string ProfilePicture { get; set; }

        public List<TweetVM> Tweets { get; set; }

        public List<FollowingUserVM> FollowingUsers { get; set; }

        public bool IsViewersProfile { get; set; }
        public bool ViewerAlreadyFollowing { get; set; }
    }
}
