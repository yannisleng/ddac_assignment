using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class TransitionModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [StringLength(50)]
        public Guid TransitionId { get; set; }

        [ForeignKey("ShipmentModel")]
        public Guid ShipmentId { get; set; }
        public virtual ShipmentModel Shipment { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Timestamp { get; set; }
    }
}
