using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Tweetus.Web.ViewModels
{
    public class PostTweetVM
    {
        public string Content { get; set; }
        public IFormFile File { get; set; }
    }
}
