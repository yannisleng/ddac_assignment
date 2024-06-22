using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ddat_assignment.Controllers.Admin
{
    public class ShipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
