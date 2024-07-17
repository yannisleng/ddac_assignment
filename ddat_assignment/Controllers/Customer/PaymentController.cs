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
        // Action method to process the payment
        public async Task<IActionResult> ProcessPayment(PaymentMethodViewModel viewModel)
        {
            // Retrieve the shipment by its ID
            var shipmentId = viewModel.ShipmentId;
            var shipment = await _context.ShipmentModel.FindAsync(shipmentId);

            // Create a new payment model with the view model data
            var payment = new PaymentModel()
            {
                ShipmentId = shipment?.ShipmentId,
                Shipment = shipment,
                Amount = viewModel.ShipmentFee,
                PaymentStatus = "Completed",
                PaymentDate = DateTime.Now,
                PaymentMethod = viewModel.PaymentMethod
            };

            // Add the payment to the context
            _context.PaymentModel.Add(payment);
            // Save the changes to the context
            await _context.SaveChangesAsync();

            // Redirect to the AirBill action in the Customer controller, passing the shipment ID
            return RedirectToAction("AirBill", "Customer", new { id = shipment.ShipmentId });
        }

    }
}
