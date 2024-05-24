using Microsoft.AspNetCore.Mvc;

namespace Bullows.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
