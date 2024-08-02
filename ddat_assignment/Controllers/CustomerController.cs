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
using System;
using System.Collections.Generic;

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

        public IActionResult Index()
        {
            return View();
        }
        // Find the user details such as name, phone number, and address and pass all those information to the create shipment view
        public async Task<IActionResult> CreateShipment()
        {
            // Retrieve the user ID from the user's claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // If the user ID is not found, redirect to the home page
                return RedirectToAction("Index", "Home");
            }

            // Asynchronously find the user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            // Asynchronously get the user's detailed information
            var userDetail = await _context.UserDetailsModel
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            // If the user or their details are not found
            if (user == null || userDetail == null)
            {
                return NotFound();
            }

            // Initialize a list to hold the address parts
            var addressParts = new List<string>();

            // If the user's address is not empty, split the address into parts
            if (!string.IsNullOrEmpty(userDetail.Address))
            {
                addressParts = userDetail.Address.Split("||").Select(p => p.Trim()).ToList();
            }

            // create a new model to store the information
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
            // Validate the shipment ID
            if (string.IsNullOrWhiteSpace(searchQuery) || !Guid.TryParse(searchQuery, out var shipmentId))
            {
                ViewBag.ErrorMessage = "Invalid shipment ID.";
                return View("Index");
            }

            // Find shipment details and transitions
            ShipmentModel shipment = await SearchShipment(shipmentId);
            List<TransitionModel> transitions = await GetShipmentTransitions(shipmentId);

            // If shipment is not found, display an error message
            if (shipment == null)
            {
                ViewBag.ErrorMessage = "Shipment not found.";
                return View("Index");
            }

            // Create a model to store shipment status information
            var shipmentStatusModel = new ShipmentResultModel
            {
                Shipment = shipment,
                Transitions = transitions
            };

            // Return the view with the shipment status model
            return View("Index", shipmentStatusModel);
        }

        // Method to generate an air bill for a shipment
        public async Task<IActionResult> AirBill(Guid id)
        {
            // Retrieve shipment details by ID
            var shipment = await _context.ShipmentModel
                .Include(s => s.Parcel)
                .Include(s => s.Sender)
                .FirstOrDefaultAsync(s => s.ShipmentId == id);

            // If shipment is not found, display an error message
            if (shipment == null)
            {
                ViewBag.ErrorMessage = "Shipment not found.";
                return RedirectToAction("Index");
            }

            // Create a view model to store air bill information
            var viewModel = new AirBillViewModel
            {
                Shipment = shipment,
                SenderName = shipment.SenderName,
                SenderPhoneNumber = shipment.SenderPhoneNumber,
                SenderAddress = shipment.PickupAddress.Replace("||", ", ")
            };

            // Return the view with the air bill view model
            return View(viewModel);
        }

        // Method to display the shipment history for the logged-in user
        public async Task<IActionResult> ShipmentHistory()
        {
            // Retrieve the user ID from the user's claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Fetch all shipments sent by the user
            var shipments = await _context.ShipmentModel
                .Where(s => s.SenderId == userId)
                .ToListAsync();

            // If no shipments are found, display an error message
            if (!shipments.Any())
            {
                ViewBag.ErrorMessage = "No shipments found for your account.";
            }

            // Return the view with the list of shipments
            return View(shipments);
        }

        // Method to check the shipping status of a specific shipment
        [HttpGet]
        public async Task<IActionResult> CheckShippingStatus(Guid id)
        {
            // Validate the shipment ID
            if (id == Guid.Empty)
            {
                TempData["error"] = "Invalid Shipment ID!";
                return RedirectToAction("Index");
            }

            // Find shipment details and transitions
            ShipmentModel shipment = await SearchShipment(id);
            List<TransitionModel> transitions = await GetShipmentTransitions(id);

            // If shipment is not found, display an error message
            if (shipment == null)
            {
                TempData["error"] = "Shipment not found!";
                return RedirectToAction("Index");
            }

            // Create a model to store shipment status information
            var shipmentStatusModel = new ShipmentResultModel
            {
                Shipment = shipment,
                Transitions = transitions
            };

            // Return the view with the shipment status model
            return View(shipmentStatusModel);
        }

        // Helper method to search for a shipment by its ID
        private async Task<ShipmentModel> SearchShipment(Guid shipmentId)
        {
            // Find shipment details by ID
            var shipment = await _context.ShipmentModel
                .Include(s => s.Parcel)
                .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

            // If shipment is found, format the addresses
            if (shipment != null)
            {
                shipment.PickupAddress = shipment.PickupAddress.Replace("||", ", ");
                shipment.DeliveryAddress = shipment.DeliveryAddress.Replace("||", ", ");
                return shipment;
            }
            return null;
        }

        // Helper method to get the transitions for a shipment by its ID
        private async Task<List<TransitionModel>> GetShipmentTransitions(Guid shipmentId)
        {
            // Fetch transitions for the shipment
            var transitionModels = await _context.TransitionModel
                .Where(t => t.ShipmentId == shipmentId)
                .ToListAsync();

            // Order transitions by timestamp and return the list
            var uniqueTransitions = transitionModels.OrderByDescending(t => t.Timestamp).ToList();
            return uniqueTransitions;
        }
    }
}