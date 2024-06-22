using Microsoft.AspNetCore.Mvc;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ddat_assignment.Controllers.Admin
{
    [Route("Admin/ManageDrivers/{action}")]
    public class ManageDriversController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public ManageDriversController(ddat_assignmentContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<DriverModel> driverModels = await _context.DriverModel.ToListAsync();
            foreach (var driverModel in driverModels)
            {
                driverModel.User = await _context.Users.FirstOrDefaultAsync(user => user.Id == Convert.ToString(driverModel.DriverId));
            }
            Debug.WriteLine("Drivers:" + driverModels);
            return View();
        }
    }
}
