using ddat_assignment.Areas.Identity.Data;
using ddat_assignment.Data;
using ddat_assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ddat_assignment.Controllers
{
	[Authorize(Roles = "Driver")]
	public class DriverController : Controller
	{
		private readonly UserManager<ddat_assignmentUser> _userManager;
		private readonly ddat_assignmentContext _context;

		public DriverController(UserManager<ddat_assignmentUser> userManager, ddat_assignmentContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var shipmentSlotModels = await GetShipmentSlotModels(GetWeekOfFirstDate(), GetWeekOfLastDate());
			return View(shipmentSlotModels);
		}

		public IActionResult Shipment()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> LoadShipmentSchedule(DateTime startDate, DateTime endDate)
		{
			var shipmentSlotModels = await GetShipmentSlotModels(startDate, endDate);
			return PartialView("_ShipmentSchedulePartial", shipmentSlotModels);
		}

		private async Task<List<ShipmentSlotModel>> GetShipmentSlotModels(DateTime startDate, DateTime endDate)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return new List<ShipmentSlotModel>();
			}

			var driver = await _context.DriverModel.FirstOrDefaultAsync(d => d.User.Id == user.Id);
			if (driver == null)
			{
				return new List<ShipmentSlotModel>();
			}

			var shipmentSlotModels = await _context.ShipmentSlotModel
				.Where(ss => ss.DriverId == driver.DriverId && ss.ShipmentDate >= startDate && ss.ShipmentDate <= endDate.AddDays(1).AddTicks(-1))
				.OrderBy(ss => ss.ShipmentDate)
				.ToListAsync();

			return shipmentSlotModels;
		}

		private DateTime GetWeekOfFirstDate()
		{
			DateTime today = DateTime.Today;

			int daysUntilStartOfWeek = ((int)today.DayOfWeek - (int)DayOfWeek.Monday);

			if (daysUntilStartOfWeek < 0)
			{
				daysUntilStartOfWeek += 7;
			}

			return today.AddDays(-daysUntilStartOfWeek);
		}

		private DateTime GetWeekOfLastDate()
		{
			DateTime today = DateTime.Today;

			int daysUntilEndOfWeek = 6 - (int)today.DayOfWeek;

			if (daysUntilEndOfWeek == 0)
			{
				daysUntilEndOfWeek += 7; 
			}

			return today.AddDays(daysUntilEndOfWeek);
		}
	}
}
