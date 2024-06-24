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
                shipment.PickupAddress = shipment.PickupAddress.Replace("||", ", ");
                shipment.DeliveryAddress = shipment.DeliveryAddress.Replace("||", ", ");
                shipment.Parcel = await _context.ParcelModel.FirstOrDefaultAsync(p => p.ParcelId == shipment.ParcelId);
            }
            shipmentModels = shipmentModels.OrderBy(s => s.ShipmentDate).ToList();
            ViewData["status"] = selectedStatus;
            return View(shipmentModels);
        }

        [HttpGet]
        public async Task<IActionResult> Workspace()
        {
            string filteredState = Request.Query["filteredState"];
            if (filteredState == null) filteredState = "Selangor";
            ViewBag.State = filteredState;
            
            List<ShipmentModel> shipmentModels = await _context.ShipmentModel.ToListAsync();
            shipmentModels = shipmentModels.Where(s => s.ShipmentStatus == "In transit" || s.ShipmentStatus == "Pending").ToList();
            shipmentModels = shipmentModels.Where(s => s.PickupAddress.Contains(filteredState)).ToList();
            shipmentModels = shipmentModels.OrderBy(s => s.ShipmentDate).ToList();
            return View(shipmentModels);
        }

        public IActionResult StandardShipping()
        {
            return View();
        }

        public IActionResult ParcelReturning()
        {
            string searchQuery = Request.Query["searchQuery"];
            if (searchQuery == null || searchQuery == "")
            {
                return View();
            }
            Guid searchQueryUuid;
            try
            {
                searchQueryUuid = Guid.Parse(searchQuery);
            }
            catch (Exception)
            {
                TempData["error"] = "Invalid Shipment ID!";
                return View();
            }
            ShipmentModel shipment = SearchShipment(searchQueryUuid).Result;
            if (shipment == null)
            {
                TempData["error"] = "Shipment with Shipment Id: " + searchQuery + " not found!";
                return View();
            }
            if (shipment.ShipmentStatus == "Returned")
            {
                TempData["error"] = "Shipment with Shipment Id: " + searchQuery + " has been returned!";
                return View();
            }
            if (shipment.ShipmentStatus != "Completed")
            {
                TempData["error"] = "Shipment with Shipment Id: " + searchQuery + " haven't been completed!";
                return View();
            }
            return View(shipment);
        }

        [HttpGet]
        public async Task<IActionResult> Shipment()
        {
            string searchQuery = Request.Query["searchQuery"]!;
            Guid searchQueryUuid;
            try
            {
                searchQueryUuid = Guid.Parse(searchQuery);
            }
            catch (Exception)
            {
                TempData["error"] = "Invalid Shipment ID!";
                return View();
            }
            ShipmentModel shipment = SearchShipment(searchQueryUuid).Result;
            if (shipment == null) TempData["error"] = "Shipment with Shipment Id: " + searchQuery + " not found!";
            return View(shipment);
        }

        public async Task<ShipmentModel> SearchShipment(Guid shipmentId)
        {
            ShipmentModel shipment = await _context.ShipmentModel.FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);
            if (shipment != null)
            {
                shipment.PickupAddress = shipment.PickupAddress.Replace("||", ", ");
                shipment.DeliveryAddress = shipment.DeliveryAddress.Replace("||", ", ");
                shipment.Parcel = await _context.ParcelModel.FirstOrDefaultAsync(p => p.ParcelId == shipment.ParcelId);
                return shipment;
            }
            return null;
        }

        public IActionResult FilterShipmentStatus(string parameter)
        {
            selectedStatus = parameter;
            return RedirectToAction("Index");
        }
    }
}
