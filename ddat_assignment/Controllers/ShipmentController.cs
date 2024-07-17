using ddat_assignment.Data;
using ddat_assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ddat_assignment.Controllers
{
    public class ShipmentController : Controller
    {
        private readonly ddat_assignmentContext _context;
        
        public ShipmentController(ddat_assignmentContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
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
            ShipmentResultModel shipmentResultModel = new ShipmentResultModel
            {
                Shipment = SearchShipment(searchQueryUuid).Result,
                Transitions = GetShipmentTransitions(searchQueryUuid).Result
            };
            if (shipmentResultModel.Shipment == null)
            {
                TempData["error"] = "Shipment with Shipment Id: " + searchQuery + " not found!";
                return View();
            }
            return View(shipmentResultModel);
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

        public async Task<List<TransitionModel>> GetShipmentTransitions(Guid shipmentId)
        {
            List<TransitionModel> transitions = await _context.TransitionModel.Where(t => t.ShipmentId == shipmentId).ToListAsync();
            transitions = transitions.OrderByDescending(t => t.Timestamp).ToList();
            return transitions;
        }
    }
}
