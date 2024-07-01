namespace ddat_assignment.Models
{
    public class DriverScheduleModel
    {
        List<ShipmentSlotModel> todaySlots;
        List<ShipmentSlotModel> tmrSlots;

        public List<ShipmentSlotModel> TodaySlots { get => todaySlots; set => todaySlots = value; }
        
        public List<ShipmentSlotModel> TmrSlots { get => tmrSlots; set => tmrSlots = value; }
    }
}
