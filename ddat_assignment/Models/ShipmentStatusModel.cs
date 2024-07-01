using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ddat_assignment.Models
{
    public class ShipmentStatusModel
    {
        public ShipmentModel Shipment { get; set; }
        public List<TransitionModel> Transitions { get; set; }
    }
}
