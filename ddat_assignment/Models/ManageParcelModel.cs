using ddat_assignment.Models;
using ddat_assignment.Data;

namespace ddat_assignment.Models
{
    public class ManageParcelModel
    {
        List<ShipmentModel> shipments = null;
        List<DriverModel> drivers = null;

        public List<ShipmentModel> Shipments { get => shipments; set => shipments = value; }

        public List<DriverModel> Drivers { get => drivers; set => drivers = value; }
    }
}
