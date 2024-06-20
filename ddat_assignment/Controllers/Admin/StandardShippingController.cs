using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Models;

namespace ddat_assignment.Controllers.Admin
{
    public class StandardShippingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /* TODO */
        [HttpPost]
        public async Task<IActionResult> CreateModels(FormCollection fc)
        {
            Console.WriteLine("CreateModels called");
            Console.WriteLine("FormCollection: " + fc);
            //get the form data
            /*var shipmentModel = new ShipmentModel
            {
                ShipmentId = Guid.NewGuid(),
            };*/
            //create the parcel model & shipping model & payment model
            //save the models
            return View();
        }
    }
}
