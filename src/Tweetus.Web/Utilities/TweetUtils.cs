using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tweetus.Web.Utilities
{
    public static class TweetUtils
    {
        public static List<string> ExtractUserMentions(string tweetText)
        {
            var result = new List<string>();

            var regex = new Regex(@"(?<=@)\w+");
            var matches = regex.Matches(tweetText);

            foreach (Match m in matches)
            {
                result.Add(m.Value);
            }

            return result;
        }
    }
}
