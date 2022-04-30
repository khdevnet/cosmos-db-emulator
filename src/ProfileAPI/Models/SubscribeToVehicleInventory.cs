using System.ComponentModel.DataAnnotations;

namespace ProfileAPI.Models;

public class SubscribeToOnSaleVehicle
{
    [Required]
    public Guid VehicleId { get; set; }
}
