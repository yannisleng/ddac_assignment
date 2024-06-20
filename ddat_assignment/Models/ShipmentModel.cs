using Microsoft.EntityFrameworkCore;
using ddat_assignment.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class ShipmentModel
    {
        [Key]
        public int ShipmentId { get; set; }

        [ForeignKey("DriverModel")]
        public int DriverId { get; set; }

        public virtual DriverModel Driver { get; set; }

        [ForeignKey("ddat_assignmentUser")]
        public int ? SenderId { get; set; }
        
        [StringLength(100)]
        public string ? SenderName { get; set; }

        public virtual ddat_assignmentUser ? Sender { get; set; }

        [ForeignKey("ddat_assignmentUser")]
        public int ? ReceiverId { get; set; }
        
        [StringLength(100)]
        public string ? ReceiverName { get; set; }

        public virtual ddat_assignmentUser ? Receiver { get; set; }

        [StringLength(255)]
        public string PickupAddress { get; set; }

        [StringLength(255)]
        public string DeliveryAddress { get; set; }

        [StringLength(50)]
        public string ShipmentStatus { get; set; }

        public DateTime ShipmentDate { get; set; }

        public DateTime? DeliveryDate { get; set; }
        
        public byte[] ? ProofOfDelivery { get; set; }

        [Precision(5, 2)]
        public decimal ? Cost { get; set; }

        [ForeignKey("ShipmentSlotModel")]
        public int ? ShipmentSlotId { get; set; }

        public virtual ShipmentSlotModel ? ShipmentSlot { get; set; }
    }
}
