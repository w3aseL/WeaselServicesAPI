using Microsoft.AspNetCore.Mvc;
using WebSocketService;

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
