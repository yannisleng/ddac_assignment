using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ddat_assignment.Models
{
    public class ParcelModel
    {
        [Key]
        public int ParcelId { get; set; }

        [Precision(5, 2)]
        public decimal Weight { get; set; }
        [Precision(10, 2)]
        public decimal Value { get; set; }

        public string Type { get; set; }
    }
}
