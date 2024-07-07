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


        public IActionResult PaymentMethod(PaymentMethodViewModel viewModel)
        {
            return View("~/Views/Customer/PaymentMethod.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentMethodViewModel viewModel)
        {

            var shipment = await _context.ShipmentModel.FindAsync(viewModel.ShipmentId);
            if (shipment == null)
            {
                ViewBag.ErrorMessage = "Shipment not found.";
                return View("PaymentMethod", viewModel);
            }

            var payment = new PaymentModel
            {
                ShipmentId = shipment.ShipmentId,
                Shipment = shipment,
                Amount = viewModel.ShipmentFee,
                PaymentStatus = "Completed",
                PaymentDate = DateTime.Now,
                PaymentMethod = viewModel.PaymentMethod
            };

            _context.PaymentModel.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment processed successfully for Shipment ID: {ShipmentId}", viewModel.ShipmentId);

            return RedirectToAction("AirBill", "Customer", new { id = shipment.ShipmentId });
        }

    }
}
