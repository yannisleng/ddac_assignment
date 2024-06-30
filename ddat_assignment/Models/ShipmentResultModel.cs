using ddat_assignment.Models;

namespace ddat_assignment.Models
{
    public class ShipmentResultModel
    {
        ShipmentModel shipment;
        List<TransitionModel> transitions;

        public ShipmentModel Shipment { get => shipment; set => shipment = value; }

        public List<TransitionModel> Transitions { get => transitions; set => transitions = value; }
    }
}
