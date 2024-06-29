using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ddat_assignment.Controllers.Admin
{
    [Authorize(Roles = "Customer")]
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
            if (ModelState.IsValid)
            {
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
                var price = Convert.ToDecimal(form["goods-price"]); // Directly get the calculated price from the form
                var shipment = new ShipmentModel
                {
                    ShipmentId = shipmentId,
                    ParcelId = parcelId,
                    Parcel = parcel,
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
                return RedirectToAction("CreateShipment", "Customer");
            }

            return RedirectToAction("CreateShipment", "Customer");
        }
    }
}
