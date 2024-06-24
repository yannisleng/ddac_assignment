using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ddat_assignment.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriverController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shipment()
        {
            return View();
        }
    }
}
