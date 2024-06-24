using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ddat_assignment.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ddat_assignmentContext _context;

        public CustomerController(ddat_assignmentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateShipment()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            var userDetail = await _context.UserDetailsModel
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            // Passing user details to the view
            ViewBag.SenderName = user.FirstName + " " + user.LastName;
            ViewBag.SenderPhoneNumber = user.PhoneNumber;
            ViewBag.SenderAddress = userDetail.Address;

            // Creating lists for states and payment methods
            List<string> states = new List<string> { "Selangor", "Sabah", "Sarawak", "Peris", "Penang", "Perak", "Johor", "Kelantan", "Kuantan", "Kuala Lumpur", "Kedah", "Melaka", "Negeri Sembilan", "Pahang", "Terengganu" };
            List<string> paymentMethods = new List<string> { "Cash", "Credit Card", "Debit Card", "E-wallet", "Online Transfer" };

            ViewBag.States = states;
            ViewBag.PaymentMethods = paymentMethods;

            return View();
        }
    }
}
