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
                var parcel = new ParcelModel();
                parcel.ParcelId = parcelId;
                parcel.GoodsName = fc["goods-name"]!;
                parcel.Weight = Convert.ToDecimal(fc["goods-weight"]);
                parcel.Value = Convert.ToDecimal(fc["goods-value"]);
                parcel.Type = fc["goods-type"]!;
                _context.ParcelModel.Add(parcel);

                //create shipment model
                var shipmentId = Guid.NewGuid();
                var shipment = new ShipmentModel();
                shipment.ShipmentId = shipmentId;
                shipment.ParcelId = parcelId;
                shipment.SenderName = fc["sender-name"]!;
                shipment.SenderPhoneNumber = fc["sender-phone-number"]!;
                shipment.ReceiverName = fc["receiver-name"]!;
                shipment.ReceiverPhoneNumber = fc["receiver-phone-number"]!;
                var pickupAddress = fc["sender-address-line-1"] + "||" +
                    fc["sender-address-line-2"] ?? "" + (Convert.ToBoolean(fc["sender-address-line-2"]) ? "" : "" + "||" ) +
                    fc["sender-postcode"] + "||" + fc["sender-city"] + "||" + fc["sender-state"];
                var deliveryAddress = fc["receiver-address-line-1"] + "||" +
                    fc["receiver-address-line-2"] ?? "" + (Convert.ToBoolean(fc["receiver-address-line-2"]) ? "" : "" + "||") +
                    fc["receiver-postcode"] + "||" + fc["receiver-city"] + "||" + fc["receiver-state"];
                shipment.PickupAddress = pickupAddress;
                shipment.DeliveryAddress = deliveryAddress;
                shipment.ShipmentStatus = "Pending";
                shipment.ShipmentDate = DateTime.Now;
                var parcelCost = Convert.ToDecimal(Convert.ToDouble(fc["goods-weight"]) * 4.5);
                shipment.Cost = parcelCost;
                _context.ShipmentModel.Add(shipment);

                //create payment model
                var payment = new PaymentModel();
                payment.ShipmentId = shipmentId;
                payment.Amount = parcelCost;
                payment.PaymentStatus = "Pending";
                payment.PaymentDate = DateTime.Now;
                payment.PaymentMethod = fc["payment-method"]!;
                _context.PaymentModel.Add(payment);

                await _context.SaveChangesAsync();
                return RedirectToAction("StandardShipping", "Admin");
            }

            return RedirectToAction("StandardShipping", "Admin");
        }
    }
}
