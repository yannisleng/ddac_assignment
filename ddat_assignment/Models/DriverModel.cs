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

        // Driving License Information
        public string LicenseId { get; set; }

        public byte[]? LicenseImage { get; set; }

        [StringLength(50)]
        public string? DrivingLicenseType { get; set; }

        public DateTime? DrivingLicenseExpiryDate { get; set; }

        [StringLength(50)]
        public string? VehicleType { get; set; }

        [StringLength(50)]
        public string? VehiclePlateNumber { get; set; }

        // Work Preferences
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [StringLength(50)]
        public string? PreferredWorkingDay { get; set; }

        [StringLength(100)]
        public string? PreferredWorkingLocation { get; set; }

        // Emergency Contact
        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(50)]
        public string? EmergencyContactRelationship { get; set; }

        public virtual ddat_assignmentUser User { get; set; }
    }
}
