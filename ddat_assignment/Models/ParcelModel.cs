using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class ParcelModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [StringLength(50)]
        public Guid ParcelId { get; set; }

        [StringLength(255)]
        public string GoodsName { get; set; }

        [Precision(5, 2)]
        public decimal Weight { get; set; }
        [Precision(10, 2)]
        public decimal Value { get; set; }

        [StringLength(20)]
        public string Type { get; set; }
    }
}
