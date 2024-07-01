using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ddat_assignment.Areas.Identity.Data;

namespace ddat_assignment.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ddat_assignmentContext _context;
        private readonly UserManager<ddat_assignmentUser> _userManager;

        public CustomerController(ddat_assignmentContext context, UserManager<ddat_assignmentUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }


        public async Task<IActionResult> CreateShipment()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            var userDetail = await _context.UserDetailsModel
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null || userDetail == null)
            {
                return NotFound();
            }

            var addressParts = userDetail.Address.Split(",").Select(p => p.Trim()).ToList();

            var viewModel = new CreateShipmentModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AddressLine1 = addressParts.ElementAtOrDefault(0) ?? "",
                AddressLine2 = addressParts.ElementAtOrDefault(1) ?? "",
                Postcode = addressParts.ElementAtOrDefault(2) ?? "",
                City = addressParts.ElementAtOrDefault(3) ?? "",
                State = addressParts.ElementAtOrDefault(4) ?? "",
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Shipment(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery) || !Guid.TryParse(searchQuery, out var shipmentId))
            {
                ViewBag.ErrorMessage = "Invalid shipment ID.";
                return View("Index");
            }

            var shipment = await _context.ShipmentModel
                .Where(s => s.ShipmentId == shipmentId)
                .Select(s => new ShipmentModel
                {
                    ShipmentId = s.ShipmentId,
                    ParcelId = s.ParcelId,
                    PickupAddress = s.PickupAddress,
                    DeliveryAddress = s.DeliveryAddress,
                    ShipmentDate = s.ShipmentDate,
                    ShipmentStatus = s.ShipmentStatus
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                ViewBag.ErrorMessage = "Shipment not found.";
                return View("Index");
            }

            return View("Index", shipment);
        }


        public async Task<IActionResult> AirBill(Guid id)
        {
            var shipment = await _context.ShipmentModel
                .Include(s => s.Parcel)
                .Include(s => s.Sender)
                .FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                ViewBag.ErrorMessage = "Shipment not found.";
                return RedirectToAction("Index");
            }

            // Add this to log or debug the retrieved data
            Console.WriteLine($"SenderName: {shipment.SenderName}, SenderPhoneNumber: {shipment.SenderPhoneNumber}, PickupAddress: {shipment.PickupAddress}");

            var viewModel = new AirBillViewModel
            {
                Shipment = shipment,
                SenderName = shipment.SenderName,
                SenderPhoneNumber = shipment.SenderPhoneNumber,
                SenderAddress = shipment.PickupAddress.Replace("||", ", ")
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ShipmentHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var shipments = await _context.ShipmentModel
                .Where(s => s.SenderId == userId)
                .ToListAsync();

            if (!shipments.Any())
            {
                ViewBag.ErrorMessage = "No shipments found for your account.";
            }

            return View(shipments);
        }

        [HttpGet]
        public async Task<IActionResult> CheckShippingStatus(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "Invalid Shipment ID!";
                return RedirectToAction("Index");
            }

            ShipmentModel shipment = await SearchShipment(id);
            List<TransitionModel> transitions = await GetShipmentTransitions(id);

            if (shipment == null)
            {
                TempData["error"] = "Shipment not found!";
                return RedirectToAction("Index");
            }

            var shipmentStatusModel = new ShipmentResultModel
            {
                Shipment = shipment,
                Transitions = transitions
            };

            return View(shipmentStatusModel);
        }

        private async Task<ShipmentModel> SearchShipment(Guid shipmentId)
        {
            var shipment = await _context.ShipmentModel
                .Include(s => s.Parcel)
                .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

            if (shipment != null)
            {
                shipment.PickupAddress = shipment.PickupAddress.Replace("||", ", ");
                shipment.DeliveryAddress = shipment.DeliveryAddress.Replace("||", ", ");
                return shipment;
            }
            return null;
        }

        private async Task<List<TransitionModel>> GetShipmentTransitions(Guid shipmentId)
        {
            var transitionModels = await _context.TransitionModel
                .Where(t => t.ShipmentId == shipmentId)
                .ToListAsync();

            var uniqueTransitions = transitionModels
                .GroupBy(t => t.Status)
                .Select(g => g.OrderByDescending(t => t.Timestamp).First())
                .ToList();

            return uniqueTransitions;
        }
    }
}
