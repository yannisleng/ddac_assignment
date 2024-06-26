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
            List<ShipmentModel> shipmentModels = await _context.ShipmentModel.Include(s => s.Parcel).ToListAsync();
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
            ManageParcelModel manageParcelModel = new ManageParcelModel();
            await GetFilteredShipmentsAndDriversData(filteredState, manageParcelModel);

            return View(manageParcelModel);
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
            ShipmentModel shipment = await _context.ShipmentModel.Include(s => s.Parcel).FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);
            if (shipment != null)
            {
                shipment.PickupAddress = shipment.PickupAddress.Replace("||", ", ");
                shipment.DeliveryAddress = shipment.DeliveryAddress.Replace("||", ", ");
                return shipment;
            }
            return null;
        }

        public IActionResult FilterShipmentStatus(string parameter)
        {
            selectedStatus = parameter;
            return RedirectToAction("Index");
        }

        public async Task<ManageParcelModel> GetFilteredShipmentsAndDriversData(string filteredState, ManageParcelModel manageParcelModel)
        {
            List<ShipmentModel> shipmentModels = await GetInTransitShipment(filteredState);
            List<DriverModel> driverModels = await GetAvailableDrivers(filteredState);

            manageParcelModel.Shipments = shipmentModels;
            manageParcelModel.Drivers = driverModels;

            return manageParcelModel;
        }

        public async Task<List<ShipmentModel>> GetInTransitShipment(string filteredState)
        {
            List<ShipmentModel> shipmentModels = await _context.ShipmentModel.Include(s => s.Parcel).ToListAsync();
            shipmentModels = shipmentModels.Where(s => s.ShipmentStatus == "Pending").ToList();
            shipmentModels = shipmentModels.Where(s => s.PickupAddress.Contains(filteredState)).ToList();
            shipmentModels = shipmentModels.OrderBy(s => s.ShipmentDate).ToList();

            List<TransitionModel> transitionModels = await _context.TransitionModel.ToListAsync();
            //remove the duplicated transitions, get the latest transition for each shipment
            List<TransitionModel> tempTransitionModels = new List<TransitionModel>();
            foreach (var transition in transitionModels)
            {
                if (tempTransitionModels.Any(t => t.ShipmentId == transition.ShipmentId))
                {
                    TransitionModel tempTransition = tempTransitionModels.First(t => t.ShipmentId == transition.ShipmentId);
                    if (tempTransition.Timestamp < transition.Timestamp)
                    {
                        tempTransitionModels.Remove(tempTransition);
                        tempTransitionModels.Add(transition);
                    }
                }
                else tempTransitionModels.Add(transition);
            }
            transitionModels = tempTransitionModels;
            transitionModels = transitionModels.Where(t => t.Status == "In Transit").ToList();
            transitionModels = transitionModels.Where(t => t.Address.Contains(filteredState)).ToList();
            foreach (var transition in transitionModels)
            {
                transition.Shipment = await _context.ShipmentModel.FirstOrDefaultAsync(s => s.ShipmentId == transition.ShipmentId);
                if (transition.Shipment!.ShipmentStatus == "In Transit")
                {
                    shipmentModels.Add(transition.Shipment);
                }
            }
            
            List<ShipmentSlotModel> shipmentSlotModels = await _context.ShipmentSlotModel.ToListAsync();
            DateTime tmrDate = DateTime.Now.AddDays(1).Date;
            shipmentSlotModels = shipmentSlotModels.Where(s => s.ShipmentDate == tmrDate).ToList();
            foreach (var shipmentSlot in shipmentSlotModels)
            {
                //remove the shipment that already assigned to the driver
                List<Guid> shipmentIds = shipmentSlot.ShipmentIds!;
                foreach (var shipmentId in shipmentIds)
                {
                    shipmentModels = shipmentModels.Where(s => s.ShipmentId != shipmentId).ToList();
                }
            }
            return shipmentModels;
        }

        public async Task<List<DriverModel>> GetAvailableDrivers(string filteredState)
        {
            List<DriverModel> driverModels = await _context.DriverModel.Include(d => d.User).ToListAsync();
            driverModels = driverModels.Where(d => d.PreferredWorkingLocation!.Contains(filteredState)).ToList();
            string tmr = DateTime.Now.AddDays(1).DayOfWeek.ToString();
            if (tmr == "Saturday" || tmr == "Sunday")
                driverModels = driverModels.Where(d => d.PreferredWorkingDay == "Weekend").ToList();
            else
                driverModels = driverModels.Where(d => d.PreferredWorkingDay == "Weekday").ToList();
            DateTime tmrDate = DateTime.Now.AddDays(1).Date;
            
            //check the shipmentslot by ShipmentDate to check the driver is available or not
            List<ShipmentSlotModel> shipmentSlotModels = await _context.ShipmentSlotModel.Include(s => s.Driver).ToListAsync();
            shipmentSlotModels = shipmentSlotModels.Where(s => s.ShipmentDate == tmrDate).ToList();
            foreach (var shipmentSlot in shipmentSlotModels)
            {
                driverModels = driverModels.Where(d => d.DriverId != shipmentSlot.DriverId).ToList();
            }
            return driverModels;
        }
    }
}
