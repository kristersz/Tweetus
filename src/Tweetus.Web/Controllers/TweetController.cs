using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.Services.Mappers;
using Tweetus.Web.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Tweetus.Web.Controllers
{
    public class TweetController : BaseController
    {
        private readonly TweetManager _tweetManager;

        public TweetController(UserManager<ApplicationUser> userManager, TweetManager tweetManager) : base(userManager)
        {
            _tweetManager = tweetManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetTweets(string userId)
        {
            var tweets = new List<TweetVM>();
            var allTweets = await _tweetManager.GetTweetsByUserId(new ObjectId(User.GetUserId()));

            if (allTweets.Count() > 0)
            {
                foreach (var t in allTweets)
                {
                    var user = await _userManager.FindByIdAsync(t.UserId.ToString());
                    tweets.Add(TweetMapper.MapTweetToViewModel(t, user));
                }
            }

            return Json(tweets);
        }

        [HttpPost]
        [Produces("text/plain")]
        public async Task<JsonResult> PostTweet(PostTweetVM tweet)
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            var now = DateTime.Now;
            var file = tweet.File;

            var newTweet = new Tweet()
            {
                Content = tweet.Content,
                UserId = new ObjectId(User.GetUserId()),
                CreatedOn = now
            };

            if (file != null)
            {
                var allowedExtensions = new string[] { ".jpg", ".png", ".gif" };

                if (file.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    var filename = parsedContentDisposition.FileName.Trim('"');

                    string ext = Path.GetExtension(filename).ToLower();

                    if (allowedExtensions.Contains(ext))
                    {
                        using (var reader = file.OpenReadStream())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                byte[] buffer = new byte[16 * 1024];

                                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ms.Write(buffer, 0, read);
                                }

                                newTweet.FileContent = ms.ToArray();
                                newTweet.FileMimeType = file.ContentType;
                                newTweet.FileOriginalName = filename;
                            }
                        }
                    }
                }
            }                                 

            await _tweetManager.CreateTweet(newTweet);

            var viewModel = new TweetVM();

            viewModel.Content = tweet.Content;
            viewModel.TweetedByName = user.FullName;
            viewModel.TweetedByHandle = user.UserName;
            viewModel.TweetedOn = now;

            if (file != null)
            {
                viewModel.ImageBase64 = Convert.ToBase64String(newTweet.FileContent);
                viewModel.ImageMimeType = file.ContentType;
            }

            return Json(viewModel);
        }
    }
}
