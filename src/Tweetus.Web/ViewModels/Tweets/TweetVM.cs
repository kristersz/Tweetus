using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.ViewModels
{
    public class TweetVM
    {
        public string TweetId { get; set; }
        public string Content { get; set; }
        public string ImageBase64 { get; set; }
        public string ImageMimeType { get; set; }
        public string TweetedByUserId { get; set; }        
        public string TweetedByFullName { get; set; }
        public string TweetedByUserName { get; set; }
        public DateTime TweetedOn { get; set; }
        public string RetweetedFromUserName { get; set; }

        public bool CanRetweet { get; set; }
        public bool CurrentUserAlreadyLiked { get; set; }
    }
}
