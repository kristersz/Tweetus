using System;

namespace Tweetus.Web.ViewModels
{
    public class TweetVM
    {
        public string TweetId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string ImageMimeType { get; set; }
        public string TweetedByUserId { get; set; }        
        public string TweetedByFullName { get; set; }
        public string TweetedByUserName { get; set; }
        public DateTime TweetedOn { get; set; }
        public string RetweetedFromUserName { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }

        public bool CanRetweet { get; set; }
        public bool CurrentUserAlreadyLiked { get; set; }
    }
}
