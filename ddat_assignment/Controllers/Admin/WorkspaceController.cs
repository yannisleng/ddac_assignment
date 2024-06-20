using Microsoft.AspNetCore.Mvc;

namespace ddat_assignment.Controllers.Admin
{
    public class WorkspaceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
