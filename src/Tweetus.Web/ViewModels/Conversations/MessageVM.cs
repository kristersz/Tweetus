using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.ViewModels
{
    public class MessageVM
    {
        public string MessageId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime SentOn { get; set; }
    }
}
