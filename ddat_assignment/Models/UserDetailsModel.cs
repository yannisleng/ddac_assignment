using ddat_assignment.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class UserDetailsModel
    {
        [Key]
        public int UserDetailsId { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [ForeignKey("ddat_assignmentUser")]
        public string UserId { get; set; }

        public virtual ddat_assignmentUser User { get; set; }
    }
}