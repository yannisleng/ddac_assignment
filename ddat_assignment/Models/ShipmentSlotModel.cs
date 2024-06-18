using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class ShipmentSlotModel
    {
        [Key]
        public int ShipmentSlotId { get; set; }

        public DateTime ShipmentDate { get; set; }

        [StringLength(50)]
        public string SlotTime { get; set; }

        [ForeignKey("DriverModel")]
        public int DriverId { get; set; }

        public virtual DriverModel Driver { get; set; }
    }
}
