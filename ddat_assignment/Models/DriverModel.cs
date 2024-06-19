using ddat_assignment.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class DriverModel
    {
        [Key]
        [ForeignKey("ddat_assignmentUser")]
        public int DriverId { get; set; }

        [Required]
        [StringLength(50)]
        [Remote("IsLicenseIDExist", "Driver", AdditionalFields = "DriverId", ErrorMessage = "License ID already exists")]
        public string LicenseId { get; set; }

        public byte[]? LicenseImage { get; set; }

        [StringLength(50)]
        public string VehicleType { get; set; }

        [StringLength(50)]
        public string VehiclePlateNumber { get; set; }

        public virtual ddat_assignmentUser User { get; set; }
    }
}
