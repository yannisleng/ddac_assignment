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
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ddat_assignmentContext context, ILogger<PaymentController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult PaymentMethod(PaymentMethodViewModel viewModel)
        {
            return View("~/Views/Customer/PaymentMethod.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentMethodViewModel viewModel)
        {
            _logger.LogInformation("Received PaymentMethodViewModel: {@viewModel}", viewModel);

            if (viewModel == null)
            {
                _logger.LogError("ViewModel is null");
                ViewBag.ErrorMessage = "Invalid payment details.";
                return View("PaymentMethod", viewModel);
            }

            // Additional null checks for properties
            if (viewModel.ShipmentId == Guid.Empty)
            {
                _logger.LogError("Shipment ID is null or empty");
                ViewBag.ErrorMessage = "Invalid shipment ID.";
                return View("PaymentMethod", viewModel);
            }
            if (viewModel.ShipmentFee == 0)
            {
                _logger.LogError("Shipment Fee is zero");
                ViewBag.ErrorMessage = "Invalid shipment fee.";
                return View("PaymentMethod", viewModel);
            }
            if (string.IsNullOrWhiteSpace(viewModel.PaymentMethod))
            {
                _logger.LogError("Payment Method is null or empty");
                ViewBag.ErrorMessage = "Invalid payment method.";
                return View("PaymentMethod", viewModel);
            }

            var shipment = await _context.ShipmentModel.FindAsync(viewModel.ShipmentId);
            if (shipment == null)
            {
                _logger.LogError("Shipment not found for ID: {ShipmentId}", viewModel.ShipmentId);
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
