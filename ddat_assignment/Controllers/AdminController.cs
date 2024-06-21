using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ddat_assignment.Controllers
{
    [Authorize(Roles = "Warehouse")]
    public class AdminController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public AdminController(ddat_assignmentContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<ShipmentModel> shipments = await _context.ShipmentModel.ToListAsync();
            //for each shipments, the address property replace the "||" with ","
            foreach (ShipmentModel shipment in shipments)
            {
                shipment.PickupAddress = shipment.PickupAddress.Replace("||", ",");
                shipment.DeliveryAddress = shipment.DeliveryAddress.Replace("||", ",");
                shipment.Parcel = await _context.ParcelModel.FirstOrDefaultAsync(p => p.ParcelId == shipment.ParcelId);
            }
            shipments = shipments.OrderBy(s => s.ShipmentDate).ToList();
            return View(shipments);
        }

        public IActionResult Workspace()
        {
            return View();
        }

        public IActionResult StandardShipping()
        {
            return View();
        }

        public IActionResult ManageDrivers()
        {
            return View();
        }
    }
}
