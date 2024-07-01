using System.ComponentModel.DataAnnotations;

namespace ddat_assignment.Models
{
    public class WarehouseModel
    {
        [Key]
        public int WarehouseId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }
    }
}
