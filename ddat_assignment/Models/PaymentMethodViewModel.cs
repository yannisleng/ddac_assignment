using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ddat_assignment.Models
{
    public class PaymentMethodViewModel
    {
        public Guid ShipmentId { get; set; }
        public decimal ShipmentFee { get; set; }
        public string PaymentMethod { get; set; }
        public string ParcelName { get; set; }
        public decimal ParcelWeight { get; set; }
    }

}
