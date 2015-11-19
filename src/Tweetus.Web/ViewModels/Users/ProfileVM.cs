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
            FollowingUsers = new List<UserVM>();
            Followers = new List<UserVM>();
            LikedTweets = new List<TweetVM>();
            ProfilePicture = string.Empty;
        }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string About { get; set; }

        public string Website { get; set; }

        public string ProfilePicture { get; set; }

        public DateTime JoinedOn { get; set; }

        public List<TweetVM> Tweets { get; set; }

        public List<UserVM> FollowingUsers { get; set; }

        public List<UserVM> Followers { get; set; }

        public List<TweetVM> LikedTweets { get; set; }

        public bool IsViewersProfile { get; set; }
        public bool ViewerAlreadyFollowing { get; set; }
    }
}
