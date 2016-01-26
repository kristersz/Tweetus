using Microsoft.AspNet.Http;

namespace Tweetus.Web.ViewModels
{
    public class PostTweetVM
    {
        public string Content { get; set; }

        public bool LocationEnabled { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public IFormFile File { get; set; }
    }
}
