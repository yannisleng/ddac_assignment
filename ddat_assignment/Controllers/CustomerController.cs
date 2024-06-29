using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ddat_assignment.Models;
using ddat_assignment.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ddat_assignment.Areas.Identity.Data;

namespace ddat_assignment.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ddat_assignmentContext _context;
        private readonly UserManager<ddat_assignmentUser> _userManager;

        public CustomerController(ddat_assignmentContext context, UserManager<ddat_assignmentUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            var userDetail = await _context.UserDetailsModel
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }
            
            // Parse the address from the database
            var addressParts = userDetail.Address.Split(",");
            string addressLine1 = addressParts.Length > 0 ? addressParts[0] : "";
            string addressLine2 = addressParts.Length > 1 ? addressParts[1] : "";
            string postcode = addressParts.Length > 2 ? addressParts[2] : "";
            string city = addressParts.Length > 3 ? addressParts[3] : "";
            string state = addressParts.Length > 4 ? addressParts[4] : "";

            // Passing user details to the view
            ViewBag.SenderName = user.FullName;
            ViewBag.SenderPhoneNumber = user.PhoneNumber;
            ViewBag.SenderAddressLine1 = addressLine1;
            ViewBag.SenderAddressLine2 = addressLine2;
            ViewBag.SenderPostcode = postcode;
            ViewBag.SenderCity = city;
            ViewBag.SenderState = state;

            TempData["UserId"] = userId;

            return View();
        }
    }
}
