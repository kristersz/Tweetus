using Microsoft.AspNet.Mvc;

namespace Tweetus.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Tweetus description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Tweetus contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
