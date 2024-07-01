using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ddat_assignment.Areas.Identity.Data;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ddat_assignment.Controllers.Admin
{
    public class CreateShipmentController : Controller
    {
        private readonly ddat_assignmentContext _context;
        //private readonly UserManager<ddat_assignmentUser> _userManager;

        public CreateShipmentController(ddat_assignmentContext context)
        {
            _context = context;
            //_userManager = userManager;
        }



        [HttpPost]
        public async Task<IActionResult> AddShipment(IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var parcelId = Guid.NewGuid();
                var parcel = new ParcelModel
                {
                    ParcelId = parcelId,
                    GoodsName = form["goods-name"],
                    Weight = Convert.ToDecimal(form["goods-weight"]),
                    Value = Convert.ToDecimal(form["goods-value"]),
                    Type = form["goods-type"]
                };
                _context.ParcelModel.Add(parcel);

                var shipmentId = Guid.NewGuid();
                var price = Convert.ToDecimal(form["goods-price"]);
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
                _context.ShipmentModel.Add(shipment);

                var payment = new PaymentModel
                {
                    ShipmentId = shipmentId,
                    Shipment = shipment,
                    Amount = price,
                    PaymentStatus = "Pending",
                    PaymentDate = DateTime.Now,
                    PaymentMethod = form["payment-method"]
                };
                _context.PaymentModel.Add(payment);

                await _context.SaveChangesAsync();

                // Redirect to AirBill page with shipment ID
                return RedirectToAction("AirBill", "Customer", new { id = shipmentId });
            }

            return RedirectToAction("CreateShipment", "Customer");
        }

    }
}
