using ProfileAPI.ApplicationCore.Common.Domain;

namespace ProfileAPI.ApplicationCore.Domain.Subscriptions;

public class VehicleOnSaleSubscriptionCreatedEvent : DomainEvent
{
    public int Test { get; } = 23;
}
