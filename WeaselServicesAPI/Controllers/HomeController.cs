using Microsoft.AspNetCore.Mvc;

namespace WeaselServicesAPI.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Home()
        {
            return View();
        }
    }
}
