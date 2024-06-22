using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class ShipmentSlotModel
    {
        [Key]
        [StringLength(50)]
        public Guid ShipmentSlotId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime ? ShipmentDate { get; set; }

        [StringLength(50)]
        public string? SlotTime { get; set; }

        [ForeignKey("DriverModel")]
        public int? DriverId { get; set; }

        public virtual DriverModel? Driver { get; set; }

        [ForeignKey("ShipmentModel")]
        public List<Guid>? ShipmentIds { get; set; }

        public virtual List<ShipmentModel>? Shipments { get; set; }
    }
}
