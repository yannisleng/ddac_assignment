using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace ddat_assignment.Controllers.Admin
{
    public class CreateShipmentController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public CreateShipmentController(ddat_assignmentContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> AddShipment(IFormCollection form)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Retrieve the user ID from the user's claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    // If the user ID is not found, redirect to the home page
                    return RedirectToAction("Index", "Home");
                }

                // Generate a new GUID for the parcel
                var parcelId = Guid.NewGuid();
                // Create a new parcel model with the form data
                var parcel = new ParcelModel
                {
                    ParcelId = parcelId,
                    GoodsName = form["goods-name"],
                    Weight = Convert.ToDecimal(form["goods-weight"]),
                    Value = Convert.ToDecimal(form["goods-price"]),
                    Type = form["goods-type"]
                };
                // Add the parcel to the context
                _context.ParcelModel.Add(parcel);

                // Generate a new GUID for the shipment
                var shipmentId = Guid.NewGuid();
                var price = Convert.ToDecimal(form["goods-price"]);
                // Create a new shipment model with the form data
                var shipment = new ShipmentModel
                {
                    ShipmentId = shipmentId,
                    ParcelId = parcelId,
                    Parcel = parcel,
                    SenderId = userId,
                    SenderName = form["sender-name"],
                    SenderPhoneNumber = form["sender-phone-number"],
                    ReceiverName = form["receiver-name"],
                    ReceiverPhoneNumber = form["receiver-phone-number"],
                    ShipmentStatus = "Pending",
                    ShipmentDate = DateTime.Now,
                    PickupAddress = $"{form["sender-address-line-1"]}||{(string.IsNullOrWhiteSpace(form["sender-address-line-2"]) ? "" : form["sender-address-line-2"] + "||")}{form["sender-postcode"]}||{form["sender-city"]}||{form["sender-state"]}",
                    DeliveryAddress = $"{form["receiver-address-line-1"]}||{(string.IsNullOrWhiteSpace(form["receiver-address-line-2"]) ? "" : form["receiver-address-line-2"] + "||")}{form["receiver-postcode"]}||{form["receiver-city"]}||{form["receiver-state"]}",
                    Cost = price
                };
                // Add the shipment to the context
                _context.ShipmentModel.Add(shipment);

                // Save the changes to the context
                await _context.SaveChangesAsync(); // Save the shipment and parcel before redirecting

                // Retrieve the payment method from the form
                string paymentMethod = form["payment-method"].ToString(); // Ensure this is a single string

                // Create a view model for the payment method
                var viewModel = new PaymentMethodViewModel
                {
                    ShipmentId = shipmentId,
                    ShipmentFee = price,
                    PaymentMethod = paymentMethod,
                    ParcelName = form["goods-name"],
                    ParcelWeight = Convert.ToDecimal(form["goods-weight"])
                };
                Console.WriteLine(viewModel);

                // Redirect to the PaymentMethod action in the Payment controller, passing the view model
                return RedirectToAction("PaymentMethod", "Payment", viewModel);
            }

            // If the model state is invalid, redirect to the CreateShipment action in the Customer controller
            return RedirectToAction("CreateShipment", "Customer");
        }
    }
}
