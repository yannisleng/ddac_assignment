using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace ddat_assignment.Controllers.Admin
{
    public class StandardShippingController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public StandardShippingController(ddat_assignmentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateModels(IFormCollection fc)
        {
            if (ModelState.IsValid)
            {
                //create parcel model
                var parcelId = Guid.NewGuid();
                var parcel = new ParcelModel
                {
                    ParcelId = parcelId,
                    GoodsName = fc["goods-name"]!,
                    Weight = Convert.ToDecimal(fc["goods-weight"]),
                    Value = Convert.ToDecimal(fc["goods-value"]),
                    Type = fc["goods-type"]!
                };
                _context.ParcelModel.Add(parcel);

                //create shipment model
                var shipmentId = Guid.NewGuid();
                var shipment = new ShipmentModel
                {
                    ShipmentId = shipmentId,
                    ParcelId = parcelId,
                    Parcel = parcel,
                    SenderName = fc["sender-name"]!,
                    SenderPhoneNumber = fc["sender-phone-number"]!,
                    ReceiverName = fc["receiver-name"]!,
                    ReceiverPhoneNumber = fc["receiver-phone-number"]!,
                    ShipmentStatus = "Pending",
                    ShipmentDate = DateTime.Now,
                };
                var pickupAddress = fc["sender-address-line-1"] + "||" + (Convert.ToBoolean(fc["sender-address-line-2"] == "") ? "" : fc["sender-address-line-2"] + "||") +
                    fc["sender-postcode"] + "||" + fc["sender-city"] + "||" + fc["sender-state"];
                var deliveryAddress = fc["receiver-address-line-1"] + "||" + (Convert.ToBoolean(fc["receiver-address-line-2"] == "") ? "" : fc["receiver-address-line-2"] + "||") +
                    fc["receiver-postcode"] + "||" + fc["receiver-city"] + "||" + fc["receiver-state"];
                shipment.PickupAddress = pickupAddress;
                shipment.DeliveryAddress = deliveryAddress;
                //search the ddat_assignmentUser in Identity where the sender phone number is the same as the input
                var user = await _context.Users.Where(u => u.PhoneNumber == fc["sender-phone-number"]).FirstOrDefaultAsync();
                if (user != null) {
                    shipment.SenderId = user.Id;
                }
                var parcelCost = Convert.ToDecimal(Convert.ToDouble(fc["goods-weight"]) * 4.5);
                shipment.Cost = parcelCost;
                _context.ShipmentModel.Add(shipment);

                //create payment model
                var payment = new PaymentModel
                {
                    ShipmentId = shipmentId,
                    Shipment = shipment,
                    Amount = parcelCost,
                    PaymentStatus = "Pending",
                    PaymentDate = DateTime.Now,
                    PaymentMethod = fc["payment-method"]!
                };
                _context.PaymentModel.Add(payment);

                await _context.SaveChangesAsync();
                return RedirectToAction("StandardShipping", "Admin");
            }

            return RedirectToAction("StandardShipping", "Admin");
        }
    }
}