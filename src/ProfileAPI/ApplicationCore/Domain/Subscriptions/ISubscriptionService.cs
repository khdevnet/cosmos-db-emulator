namespace ProfileAPI.ApplicationCore.Domain.Subscriptions;

public interface ISubscriptionService
{
    Task<Subscription> SubscribeToVehicleInventory(Guid customerId, Guid vehicleId);
}
