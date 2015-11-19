using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.ViewModels
{
    public class UserVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string UserAbout { get; set; }
        public string ProfilePictureBase64 { get; set; }
    }
}
