using ddat_assignment.Areas.Identity.Data;
using ddat_assignment.Data;
using ddat_assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http.Headers;

namespace ddat_assignment.Controllers
{
	[Authorize(Roles = "Driver")]
	public class DriverController : Controller
	{
		private readonly UserManager<ddat_assignmentUser> _userManager;
		private readonly ddat_assignmentContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public DriverController(UserManager<ddat_assignmentUser> userManager, ddat_assignmentContext context, IHttpClientFactory clientFactory)
		{
			_userManager = userManager;
			_context = context;
            _clientFactory = clientFactory;
		}

		public async Task<IActionResult> Index()
		{
            // get this week's shipments
			var shipmentSlotModels = await GetShipmentSlotModels(GetWeekOfFirstDate(), GetWeekOfLastDate());
			return View(shipmentSlotModels);
		}

		public async Task<IActionResult> Shipment()
		{
            var user = await _userManager.GetUserAsync(User);

            // get driver's shipment slots
            var shipmentSlots = await _context.ShipmentSlotModel
			 .Where(ss => ss.Driver!.User.Id == user!.Id)
			 .ToListAsync();

            // get all shipment ids
            var shipmentIds = shipmentSlots.SelectMany(ss => ss.ShipmentIds!).ToList();

            // get all shipments by ids
            var shipments = await _context.ShipmentModel
                .Include(s => s.Sender)
                .Where(s => shipmentIds.Contains(s.ShipmentId))
				.OrderByDescending(s => s.ShipmentDate)
                .ToListAsync();

            // get all transitions by ids
            var transitions = await _context.TransitionModel
				.Where(t => shipmentIds.Contains(t.ShipmentId))
				.GroupBy(t => t.ShipmentId)
				.Select(g => g.OrderByDescending(t => t.Timestamp).First())
				.ToListAsync();

            // store the retrieve shipments, shipment slots, and transitions into the model
            ManageShipmentModel manageShipmentModels = new()
            {
                Shipments = shipments ?? [],
                ShipmentSlots = shipmentSlots ?? [],
                Transitions = transitions ?? new List<TransitionModel>()
            };

            // pass the model into view
            return View(manageShipmentModels);
        }

        // get shipments within a range of date to schedule partial view
		[HttpGet]
		public async Task<IActionResult> LoadShipmentSchedule(DateTime startDate, DateTime endDate)
		{
			List<ShipmentSlotModel> shipmentSlotModels = await GetShipmentSlotModels(startDate, endDate);

			return PartialView("_ShipmentSchedulePartial", shipmentSlotModels);
		}

        // get driver's shipments within a date range
		private async Task<List<ShipmentSlotModel>> GetShipmentSlotModels(DateTime startDate, DateTime endDate)
		{
			var user = await _userManager.GetUserAsync(User);

			var driver = await _context.DriverModel.FirstOrDefaultAsync(d => d.User.Id == user!.Id);

            List<ShipmentSlotModel> shipmentSlotModels = new List<ShipmentSlotModel>();

            shipmentSlotModels = await _context.ShipmentSlotModel
				.Where(ss => ss.DriverId == driver!.DriverId && ss.ShipmentDate >= startDate && ss.ShipmentDate <= endDate.AddDays(1).AddTicks(-1))
				.OrderBy(ss => ss.ShipmentDate)
				.ToListAsync();

            if (!shipmentSlotModels.Any())
            {
                shipmentSlotModels.Add(new ShipmentSlotModel
                {
                    DriverId = driver?.DriverId,
                    Driver = driver,
                });
            }

            return shipmentSlotModels;
		}

        // get the first day's date of the week
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

        // get the last day's date of the week
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

        //Shipment View
        [HttpGet]
        public async Task<IActionResult> LoadShipmentData(string searchTerm = "")
        {
            // get user details
            var user = await _userManager.GetUserAsync(User);

            // get driver's shipment slots
            var shipmentSlotsQuery = _context.ShipmentSlotModel
                .Where(ss => ss.Driver!.User.Id == user!.Id);

            // create empty shipment slot list
            var shipmentSlotsQueryResult = new List<ShipmentSlotModel>();

            // if search string is not empty
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // filter shipment slots by shipment slot id or shipment date
                shipmentSlotsQueryResult = await shipmentSlotsQuery.Where(ss =>
                    EF.Functions.Like(ss.ShipmentSlotId.ToString(), $"%{searchTerm}%") ||
                    EF.Functions.Like(ss.ShipmentDate.ToString(), $"%{searchTerm}%")
                ).ToListAsync();

                // if found
                if (shipmentSlotsQueryResult.Count > 0)
                {
                    // store the query into shipment slot query
                    shipmentSlotsQuery = shipmentSlotsQuery.Where(ss =>
                        EF.Functions.Like(ss.ShipmentSlotId.ToString(), $"%{searchTerm}%") ||
                        EF.Functions.Like(ss.ShipmentDate.ToString(), $"%{searchTerm}%")
                    );
                }
            }

            // get driver's shipment ids
            var shipmentIdsQuery = shipmentSlotsQuery
                .SelectMany(ss => ss.ShipmentIds!);

            // get transitions by shipment ids
            var transitionsQuery = _context.TransitionModel
                .Where(t => shipmentIdsQuery.Contains(t.ShipmentId));

            // store the query into shipment query
            var shipmentsQuery = _context.ShipmentModel
                .Include(s => s.Sender)
                .Where(s => shipmentIdsQuery.Contains(s.ShipmentId));

            // create empty shipment list
            var shipmentQueryResult = new List<ShipmentModel>();

            // create empty transition list
            var transitionQueryResult = new List<TransitionModel>();

            // if search string is not empty and shipment slot list is empty
            if (!string.IsNullOrEmpty(searchTerm) && shipmentSlotsQueryResult.Count == 0)
            {
                // filter shipment by shipment id, receiver name, delivery address, receiver phone number, or shipment status
                shipmentQueryResult = await shipmentsQuery.Where(s =>
                    EF.Functions.Like(s.ShipmentId.ToString(), $"%{searchTerm}%") ||
                    EF.Functions.Like(s.ReceiverName, $"%{searchTerm}%") ||
                    EF.Functions.Like(s.DeliveryAddress, $"%{searchTerm}%") ||
                    EF.Functions.Like(s.ReceiverPhoneNumber, $"%{searchTerm}%") ||
                    EF.Functions.Like(s.ShipmentStatus, $"%{searchTerm}%")
                ).ToListAsync();

                // filter transition by address
                transitionQueryResult = await transitionsQuery
                    .Where(t => EF.Functions.Like(t.Address, $"%{searchTerm}%")
                        && t.Timestamp == transitionsQuery
                        .Where(t2 => t2.ShipmentId == t.ShipmentId)
                        .Max(t2 => t2.Timestamp)
                ).ToListAsync();

                // merge shipments and transitions
                var matchingShipmentIds = shipmentQueryResult.Select(s => s.ShipmentId)
                        .Union(transitionQueryResult.Select(t => t.ShipmentId))
                        .Distinct();

                // store the query into shipment query
                shipmentsQuery = shipmentsQuery.Where(s => matchingShipmentIds.Contains(s.ShipmentId));

                // store the query into transition query
                transitionsQuery = transitionsQuery.Where(t => matchingShipmentIds.Contains(t.ShipmentId));
            }

            // final shipment slot, shipment, and transition query
            var shipmentSlots = await shipmentSlotsQuery.ToListAsync();
            var shipments = await shipmentsQuery.OrderByDescending(s => s.ShipmentDate).ToListAsync();
            var transitions = await transitionsQuery
                .GroupBy(t => t.ShipmentId)
                .Select(g => g.OrderByDescending(t => t.Timestamp).First())
                .ToListAsync();

            // store the query result into manage shipment model
            ManageShipmentModel manageShipmentModels = new ManageShipmentModel
            {
                Shipments = shipments ?? new List<ShipmentModel>(),
                ShipmentSlots = shipmentSlots ?? new List<ShipmentSlotModel>(),
                Transitions = transitions ?? new List<TransitionModel>()
            };

            // pass the manage shipment model into partial view
            return PartialView("_ShipmentTablePartial", manageShipmentModels);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShipmentStatus(Guid shipmentId, string newStatus, string transitLocation)
        {
            var shipment = await _context.ShipmentModel.FindAsync(shipmentId);

            if (newStatus == "Delivered")
            {
                shipment!.ShipmentStatus = newStatus;
                shipment!.DeliveryDate = DateTime.Now;
                transitLocation = shipment!.DeliveryAddress.Replace("||", ", ");
            }

            // Create a new transition for "Picked Up"
            if (shipment!.ShipmentStatus == "Pending")
            {
                var defaultTransistion = new TransitionModel
                {
                    ShipmentId = shipmentId,
                    Address = shipment!.PickupAddress.Replace("||", ", "),
                    Status = "Picked Up",
                    Timestamp = DateTime.Now
                };

                _context.TransitionModel.Add(defaultTransistion);
            }

            // Create a new transition for "In Transist" or "Delivered"
            var transition = new TransitionModel
            {
                ShipmentId = shipmentId,
                Address = transitLocation,
                Status = newStatus,
                Timestamp = DateTime.Now
            };

            _context.TransitionModel.Add(transition);

            // If the status is "Delivered", handle proof of delivery
            if (newStatus == "Delivered")
            {
                var file = Request.Form.Files.GetFile("proofOfDelivery");
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://4oo79wvu43.execute-api.us-east-1.amazonaws.com/s3uploadpod");
                    request.Headers.Add("filename", $"{shipmentId}.jpg");
                    request.Content = new StreamContent(file.OpenReadStream());

                    var response = await client.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Error: {errorContent}");
                    }
                }
            }

            try
            {
                shipment.ShipmentStatus = newStatus;
                _context.ShipmentModel.Update(shipment);
                await _context.SaveChangesAsync();
                return Ok("Shipment status updated and transition added successfully.");
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while updating the database.");
            }
        }
    }
}
