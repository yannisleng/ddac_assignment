using Microsoft.AspNetCore.Mvc;
using ddat_assignment.Models;
using ddat_assignment.Data;
using System.Threading.Tasks;
using System;

namespace ddat_assignment.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public PaymentController(ddat_assignmentContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult PaymentMethod()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(IFormCollection form)
        {
            var shipmentId = Guid.Parse(TempData["shipmentId"].ToString());
            var parcelId = Guid.Parse(TempData["parcelId"].ToString());
            var price = decimal.Parse(TempData["price"].ToString());
            string paymentMethod = TempData["paymentMethod"].ToString(); // Ensure this is a single string

            var shipment = await _context.ShipmentModel.FindAsync(shipmentId);
            var parcel = await _context.ParcelModel.FindAsync(parcelId);

            var payment = new PaymentModel
            {
                ShipmentId = shipmentId,
                Shipment = shipment,
                Amount = price,
                PaymentStatus = "Completed",
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod
            };
            _context.PaymentModel.Add(payment);

            await _context.SaveChangesAsync();

            TempData.Clear();

            return RedirectToAction("AirBill", "Customer", new { id = shipmentId });
        }
    }
}
