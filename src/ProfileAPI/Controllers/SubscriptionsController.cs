using ProfileAPI.ApplicationCore.Common.Domain;
using ProfileAPI.ApplicationCore.Domain;
using ProfileAPI.ApplicationCore.Domain.Subscriptions;
using ProfileAPI.ApplicationCore.Domain.Subscriptions.Specs;
using ProfileAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProfileAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly IRepository<Subscription> _subscriptionRepository;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(
        IRepository<Subscription> subscriptionRepository,
        ISubscriptionService subscriptionService)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionService = subscriptionService;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("onsale/vehicles")]
    public async Task<ActionResult> GetOnsale(Guid customerId)
    {
        var subscription = await _subscriptionRepository.GetBySpecAsync(new GetByCustomerIdSpec(customerId));

        return subscription is null
            ? NotFound()
            : Ok(subscription);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("onsale/vehicles")]
    public async Task<ActionResult> Add(
        Guid customerId,
        [FromBody] SubscribeToOnSaleVehicle subscribeToOnSaleVehicle)
        => Ok(await _subscriptionService
            .SubscribeToVehicleInventory(customerId, subscribeToOnSaleVehicle.VehicleId));
}
