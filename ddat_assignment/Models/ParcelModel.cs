using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddat_assignment.Models
{
    public class ParcelModel
    {
        [Key]
        public int ParcelId { get; set; }

        public decimal Weight { get; set; }

        public decimal Value { get; set; }

        public string Type { get; set; }
    }
}
