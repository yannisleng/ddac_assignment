namespace ddat_assignment.Models
{
    public class ManageShipmentModel
    {
        public List<ShipmentModel>? Shipments { get; set; }
        public List<ShipmentSlotModel>? ShipmentSlots { get; set; }
        public List<TransitionModel>? Transitions { get; set; }

    }
}
