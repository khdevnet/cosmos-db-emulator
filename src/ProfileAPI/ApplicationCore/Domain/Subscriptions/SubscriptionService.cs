using ProfileAPI.ApplicationCore.Common.Domain;
using ProfileAPI.ApplicationCore.Domain.Subscriptions.Specs;

namespace ProfileAPI.ApplicationCore.Domain.Subscriptions;

public class SubscriptionService : ISubscriptionService
{
    private readonly IRepository<Subscription> _subscriptionRepository;

    public SubscriptionService(IRepository<Subscription> subscriptionRepository)
        => _subscriptionRepository = subscriptionRepository;

    public async Task<Subscription> SubscribeToVehicleInventory(Guid customerId, Guid vehicleId)
    {
        var subscription = await _subscriptionRepository.GetBySpecAsync(new GetByCustomerIdSpec(customerId));
        var isNewSubscription = subscription is null;
        if (isNewSubscription)
        {
            subscription = new Subscription(customerId);
        }

        subscription.AddVehicle(vehicleId);

        if (isNewSubscription)
        {
            await _subscriptionRepository.AddAsync(subscription);
        }
        else
        {
            await _subscriptionRepository.UpdateAsync(subscription);
        }

        return subscription;
    }
}
