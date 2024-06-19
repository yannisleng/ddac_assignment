using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class PaymentModel
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("ShipmentModel")]
        public int ? ShipmentId { get; set; }

        public virtual ShipmentModel ? Shipment { get; set; }

        [Precision(10, 2)]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [StringLength(50)]
        public string ? PaymentStatus { get; set; }

        [StringLength(50)]
        public string ? PaymentMethod { get; set; }
    }
}
