﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.ViewModels
{
    public class DashboardVM
    {
        public DashboardVM()
        {
            Tweets = new List<TweetVM>();
        }

        public string ProfilePicture { get; set; }

        public long TweetCount { get; set; }
        public long FollowerCount { get; set; }
        public long FollowingCount { get; set; }
        public List<TweetVM> Tweets { get; set; }
    }
}
