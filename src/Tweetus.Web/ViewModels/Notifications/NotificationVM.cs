using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetus.Web.Data.Enums;

namespace Tweetus.Web.ViewModels
{
    public class NotificationVM
    {
        public NotificationType Type { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedOn { get; set; }

        public string TweetContent { get; set; }
        public DateTime? TweetedOn { get; set; }
    }
}
