using ddat_assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Models;
using ddat_assignment.Data;
using System.Diagnostics;

namespace ddat_assignment.Controllers.Admin
{
    public class ParcelReturningController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public ParcelReturningController(ddat_assignmentContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReturnParcel(IFormCollection fc)
        {
            Guid shipmentId = Guid.Parse(fc["shipmentId"]!);
            //create new shipment
            ShipmentModel shipment = await _context.ShipmentModel.FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);
            shipment!.Parcel = await _context.ParcelModel.FirstOrDefaultAsync(p => p.ParcelId == shipment.ParcelId);
            shipment.ShipmentStatus = "Returned";
            _context.ShipmentModel.Update(shipment);

            ShipmentModel newShipment = new()
            {
                ShipmentId = Guid.NewGuid(),
                ParcelId = shipment.ParcelId,
                Parcel = shipment.Parcel,
                SenderId = shipment.ReceiverId,
                SenderName = shipment.ReceiverName,
                SenderPhoneNumber = shipment.ReceiverPhoneNumber,
                ReceiverId = shipment.SenderId,
                ReceiverName = shipment.SenderName,
                ReceiverPhoneNumber = shipment.SenderPhoneNumber,
                PickupAddress = shipment.DeliveryAddress,
                DeliveryAddress = shipment.PickupAddress,
                ShipmentDate = DateTime.Now,
                ShipmentStatus = "Pending",
                Cost = shipment.Cost, 
            };
            _context.ShipmentModel.Add(newShipment);
            await _context.SaveChangesAsync();

            TempData["success"] = true;
            return RedirectToAction("ParcelReturning", "Admin");
        }
    }
}
