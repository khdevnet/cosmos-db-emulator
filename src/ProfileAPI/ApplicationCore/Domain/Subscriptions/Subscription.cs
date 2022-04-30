using Ardalis.GuardClauses;
using ProfileAPI.ApplicationCore.Common.Domain;
using ProfileAPI.ApplicationCore.Domain.Subscriptions;
using ProfileAPI.ApplicationCore.Exceptions;
using ProfileAPI.Common.Models;

namespace ProfileAPI.ApplicationCore.Domain;

public class Subscription : BaseEntity, IAggregateRoot
{
    public Subscription(Guid customerId, IList<Guid>? vehicleIds = default)
    {
        CustomerId = Guard.Against.NullOrEmpty(customerId);
        VehicleIds = (vehicleIds ?? Enumerable.Empty<Guid>()).ToList();
    }

    public Guid CustomerId { get; private set; }

    public IList<Guid> VehicleIds { get; private set; }

    public void AddVehicle(Guid vehicleId)
    {
        Guard.Against.NullOrEmpty(vehicleId);

        if (VehicleIds.Contains(vehicleId))
        {
            throw new DomainException(ErrorCode.SubscriptionExist, new { vehicleId });
        }

        VehicleIds.Add(vehicleId);
        Events.Add(new VehicleOnSaleSubscriptionCreatedEvent());
    }

    public bool HasVehicle(Guid vehicleId)
    {
        Guard.Against.NullOrEmpty(vehicleId);

        return VehicleIds.Contains(vehicleId);
    }

    public bool RemoveVehicle(Guid vehicleId)
    {
        Guard.Against.NullOrEmpty(vehicleId);

        return VehicleIds.Remove(vehicleId);
    }
}
