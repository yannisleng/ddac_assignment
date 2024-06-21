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
        private static string selectedStatus = "All";

        public AdminController(ddat_assignmentContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<ShipmentModel> shipmentModels = await _context.ShipmentModel.ToListAsync();
            if (selectedStatus == "All")
            { }
            else
            {
                //filter the shipment by status
                shipmentModels = shipmentModels.Where(s => s.ShipmentStatus == selectedStatus).ToList();
            }
            //for each shipments, the address property replace the "||" with ","
            foreach (ShipmentModel shipment in shipmentModels)
            {
                shipment.PickupAddress = shipment.PickupAddress.Replace("||", ",");
                shipment.DeliveryAddress = shipment.DeliveryAddress.Replace("||", ",");
                shipment.Parcel = await _context.ParcelModel.FirstOrDefaultAsync(p => p.ParcelId == shipment.ParcelId);
            }
            shipmentModels = shipmentModels.OrderBy(s => s.ShipmentDate).ToList();
            ViewData["status"] = selectedStatus;
            return View(shipmentModels);
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

        public IActionResult FilterShipmentStatus(string parameter)
        {
            selectedStatus = parameter;
            return RedirectToAction("Index");
        }
    }
}
