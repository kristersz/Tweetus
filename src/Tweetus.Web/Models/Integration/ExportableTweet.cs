using System;

namespace Tweetus.Web.Models.Integration
{
    public class ExportableTweet
    {
        public string TweetId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
