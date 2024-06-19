using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class TransitionModel
    {
        [Key]
        public int TransitionId { get; set; }

        [ForeignKey("ShipmentModel")]
        public int ShipmentId { get; set; }
        public virtual ShipmentModel Shipment { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
