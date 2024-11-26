using Microsoft.AspNetCore.Mvc;

namespace DLForum.Controllers
{
    public class DetailsController : Controller
    {
        public IActionResult DetailsAdmin()
        {
            return View();
        }
    }
}
