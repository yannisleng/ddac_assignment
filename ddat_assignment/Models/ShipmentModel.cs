using Microsoft.EntityFrameworkCore;
using ddat_assignment.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class ShipmentModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [StringLength(50)]
        public Guid ShipmentId { get; set; }


        [ForeignKey("DriverModel")]
        public int? DriverId { get; set; }

        public virtual DriverModel? Driver { get; set; }

        [ForeignKey("ParcelModel")]
        public Guid? ParcelId { get; set; }
        public virtual ParcelModel? Parcel { get; set; }

        [ForeignKey("Sender")]
        public string? SenderId { get; set; }
        public virtual ddat_assignmentUser? Sender { get; set; }

        [StringLength(100)]
        public string? SenderName { get; set; }

        [StringLength(15)]
        public string? SenderPhoneNumber { get; set; }

        [ForeignKey("Receiver")]
        public string? ReceiverId { get; set; }
        public virtual ddat_assignmentUser? Receiver { get; set; }

        [StringLength(100)]
        public string? ReceiverName { get; set; }

        [StringLength(15)]
        public string? ReceiverPhoneNumber { get; set; }

        [StringLength(255)]
        public string PickupAddress { get; set; }

        [StringLength(255)]
        public string DeliveryAddress { get; set; }

        [StringLength(50)]
        public string ShipmentStatus { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime ShipmentDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public byte[]? ProofOfDelivery { get; set; }

        [StringLength(100)]
        public string? ProofOfDeliveryFileName { get; set; }

        [StringLength(50)]
        public string? ProofOfDeliveryContentType { get; set; }

        [Precision(5, 2)]
        public decimal? Cost { get; set; }
    }
}
