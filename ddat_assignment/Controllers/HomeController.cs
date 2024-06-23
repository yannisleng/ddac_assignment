using ddat_assignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ddat_assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Warehouse"))
            {
                return LocalRedirect("~/Admin");
            }
            else if (User.IsInRole("Driver"))
            {
                return LocalRedirect("~/Driver");
            }
            else if (User.IsInRole("Customer"))
            {
                return LocalRedirect("~/Customer");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
