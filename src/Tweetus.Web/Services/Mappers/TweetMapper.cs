using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.Models;
using Tweetus.Web.ViewModels;

namespace Tweetus.Web.Services.Mappers
{
    public static class TweetMapper
    {
        public static IList<TweetVM> MapTweetsToViewModels(IList<Tweet> tweets, ApplicationUser user)
        {
            var result = new List<TweetVM>();

            foreach (var tweet in tweets)
            {
                result.Add(MapTweetToViewModel(tweet, user));
            }

            return result;
        }

        public static TweetVM MapTweetToViewModel(Tweet tweet, ApplicationUser user)
        {
            return new TweetVM()
            {
                TweetId = tweet.Id.ToString(),
                Content = tweet.Content,
                ImageBase64 = (tweet.FileContent != null) ? Convert.ToBase64String(tweet.FileContent) : string.Empty,
                ImageMimeType = (tweet.FileMimeType != null) ? tweet.FileMimeType : string.Empty,
                TweetedByUserId = user.Id,
                TweetedByFullName = user.FullName,
                TweetedByUserName = user.UserName,
                TweetedOn = tweet.CreatedOn,
                RetweetedFromUserName = tweet.RetweetedFromUserName
            };
        }
    }
}
