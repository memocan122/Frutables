using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frutables.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        
    }
}
