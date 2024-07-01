using ddat_assignment.Models;
using Microsoft.AspNetCore.Mvc;
using ddat_assignment.Data;

namespace ddat_assignment.Controllers.Admin
{
    public class WorkspaceController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public WorkspaceController(ddat_assignmentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewShipmentSlot(IFormCollection fc)
        {
            ShipmentSlotModel shipmentSlot = new()
            {
                ShipmentSlotId = Guid.NewGuid(),
                ShipmentDate = DateTime.Now.AddDays(1).Date,
                SlotTime = "8:00 AM - 5:00 PM",
                DriverId = Convert.ToInt16(fc["selectedDriverId"]),
                ShipmentIds = fc["selectedShipmentIds"].ToString().Split(',').Select(Guid.Parse).ToList()
            };
            _context.ShipmentSlotModel.Add(shipmentSlot);
            await _context.SaveChangesAsync();
            return RedirectToAction("Workspace", "Admin");
        }
    }
}
