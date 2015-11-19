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
        public string TweetedByName { get; set; }
        public string TweetedByHandle { get; set; }
        public DateTime TweetedOn { get; set; }
    }
}
