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

		public async Task<IActionResult> Shipment()
		{
            var user = await _userManager.GetUserAsync(User);

            var shipmentSlots = await _context.ShipmentSlotModel
			 .Where(ss => ss.Driver!.User.Id == user!.Id)
			 .ToListAsync();

            var shipmentIds = shipmentSlots.SelectMany(ss => ss.ShipmentIds!).ToList();

            var shipments = await _context.ShipmentModel
                .Include(s => s.Sender)
                .Include(s => s.Receiver)
                .Where(s => shipmentIds.Contains(s.ShipmentId))
				.OrderByDescending(s => s.ShipmentDate)
                .ToListAsync();

            var transitions = await _context.TransitionModel
				.Where(t => shipmentIds.Contains(t.ShipmentId))
				.GroupBy(t => t.ShipmentId)
				.Select(g => g.OrderByDescending(t => t.Timestamp).First())
				.ToListAsync();

            ManageShipmentModel manageShipmentModels = new ManageShipmentModel();

            manageShipmentModels.Shipments = shipments ?? new List<ShipmentModel>();
            manageShipmentModels.ShipmentSlots = shipmentSlots ?? new List<ShipmentSlotModel>();
            manageShipmentModels.Transitions = transitions ?? new List<TransitionModel>();

            return View(manageShipmentModels);
        }

		[HttpGet]
		public async Task<IActionResult> LoadShipmentSchedule(DateTime startDate, DateTime endDate)
		{
			List<ShipmentSlotModel> shipmentSlotModels = await GetShipmentSlotModels(startDate, endDate);

			return PartialView("_ShipmentSchedulePartial", shipmentSlotModels);
		}

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

        //Shipment View
        [HttpGet]
        public async Task<IActionResult> LoadShipmentData(string searchTerm = "")
        {
            var user = await _userManager.GetUserAsync(User);

            var shipmentSlotsQuery = _context.ShipmentSlotModel
                .Where(ss => ss.Driver!.User.Id == user!.Id);

            var shipmentSlotsQueryResult = new List<ShipmentSlotModel>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                shipmentSlotsQueryResult = await shipmentSlotsQuery.Where(ss =>
                    EF.Functions.Like(ss.ShipmentSlotId.ToString(), $"%{searchTerm}%") ||
                    EF.Functions.Like(ss.ShipmentDate.ToString(), $"%{searchTerm}%")
                ).ToListAsync();

                if (shipmentSlotsQueryResult.Count > 0)
                {
                    shipmentSlotsQuery = shipmentSlotsQuery.Where(ss =>
                        EF.Functions.Like(ss.ShipmentSlotId.ToString(), $"%{searchTerm}%") ||
                        EF.Functions.Like(ss.ShipmentDate.ToString(), $"%{searchTerm}%")
                    );
                }
            }

            var shipmentIdsQuery = shipmentSlotsQuery
                .SelectMany(ss => ss.ShipmentIds!);

            var transitionsQuery = _context.TransitionModel
                .Where(t => shipmentIdsQuery.Contains(t.ShipmentId));

            var shipmentsQuery = _context.ShipmentModel
                .Include(s => s.Sender)
                .Include(s => s.Receiver)
                .Where(s => shipmentIdsQuery.Contains(s.ShipmentId));

            var shipmentQueryResult = new List<ShipmentModel>();
            var transitionQueryResult = new List<TransitionModel>();

            if (!string.IsNullOrEmpty(searchTerm) && shipmentSlotsQueryResult.Count == 0)
            {
                shipmentQueryResult = await shipmentsQuery.Where(s =>
                    EF.Functions.Like(s.ShipmentId.ToString(), $"%{searchTerm}%") ||
                    EF.Functions.Like(s.ReceiverName, $"%{searchTerm}%") ||
                    EF.Functions.Like(s.DeliveryAddress, $"%{searchTerm}%") ||
                    EF.Functions.Like(s.ReceiverPhoneNumber, $"%{searchTerm}%") ||
                    EF.Functions.Like(s.ShipmentStatus, $"%{searchTerm}%")
                ).ToListAsync();

                transitionQueryResult = await transitionsQuery
                    .Where(t => EF.Functions.Like(t.Address, $"%{searchTerm}%")
                        && t.Timestamp == transitionsQuery
                        .Where(t2 => t2.ShipmentId == t.ShipmentId)
                        .Max(t2 => t2.Timestamp)
                ).ToListAsync();

                var matchingShipmentIds = shipmentQueryResult.Select(s => s.ShipmentId)
                        .Union(transitionQueryResult.Select(t => t.ShipmentId))
                        .Distinct();

                shipmentsQuery = shipmentsQuery.Where(s => matchingShipmentIds.Contains(s.ShipmentId));
                transitionsQuery = transitionsQuery.Where(t => matchingShipmentIds.Contains(t.ShipmentId));
            }

            var shipmentSlots = await shipmentSlotsQuery.ToListAsync();
            var shipments = await shipmentsQuery.OrderByDescending(s => s.ShipmentDate).ToListAsync();
            var transitions = await transitionsQuery
                .GroupBy(t => t.ShipmentId)
                .Select(g => g.OrderByDescending(t => t.Timestamp).First())
                .ToListAsync();

            ManageShipmentModel manageShipmentModels = new ManageShipmentModel
            {
                Shipments = shipments ?? new List<ShipmentModel>(),
                ShipmentSlots = shipmentSlots ?? new List<ShipmentSlotModel>(),
                Transitions = transitions ?? new List<TransitionModel>()
            };

            return PartialView("_ShipmentTablePartial", manageShipmentModels);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShipmentStatus(Guid shipmentId, string newStatus, string transitLocation)
        {
            var shipment = await _context.ShipmentModel.FindAsync(shipmentId);

            // Update shipment status
            shipment!.ShipmentStatus = newStatus;

            if (newStatus == "Delivered")
            {
                shipment.DeliveryDate = DateTime.Now;
            }

            // Create a new transition
            if (shipment!.ShipmentStatus == "Pending")
            {
                var defaultTransistion = new TransitionModel
                {
                    ShipmentId = shipmentId,
                    Address = shipment!.PickupAddress,
                    Status = "Picked Up",
                    Timestamp = DateTime.Now
                };

                _context.TransitionModel.Add(defaultTransistion);
            }

            if (newStatus == "Delivered")
            {
                transitLocation = shipment!.DeliveryAddress;
            }

            // Create a new transition
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
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        shipment.ProofOfDelivery = memoryStream.ToArray();
                        shipment.ProofOfDeliveryFileName = file.FileName;
                        shipment.ProofOfDeliveryContentType = file.ContentType;
                    }
                }
                else
                {
                    return BadRequest("Proof of delivery is required for 'Delivered' status.");
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Shipment status updated and transition added successfully.");
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while updating the database.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProofOfDelivery(string shipmentId)
        {
            if (!Guid.TryParse(shipmentId, out Guid parsedShipmentId))
            {
                return BadRequest("Invalid shipment ID.");
            }

            var shipment = await _context.ShipmentModel.FindAsync(parsedShipmentId);
            if (shipment?.ProofOfDelivery == null)
            {
                return NotFound();
            }

            var response = new
            {
                Image = Convert.ToBase64String(shipment.ProofOfDelivery),
                ContentType = shipment.ProofOfDeliveryContentType,
                FileName = shipment.ProofOfDeliveryFileName,
                DeliveryDate = shipment.DeliveryDate 
            };

            return Json(response);
        }
    }
}
