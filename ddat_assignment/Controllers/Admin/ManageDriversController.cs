using Microsoft.AspNetCore.Mvc;

namespace ddat_assignment.Controllers.Admin
{
    public class ManageDriversController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
