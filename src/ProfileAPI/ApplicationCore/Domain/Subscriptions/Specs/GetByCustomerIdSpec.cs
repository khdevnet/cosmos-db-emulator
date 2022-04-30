using Ardalis.Specification;

namespace ProfileAPI.ApplicationCore.Domain.Subscriptions.Specs;

public class GetByCustomerIdSpec : Specification<Subscription>, ISingleResultSpecification
{
    public GetByCustomerIdSpec(Guid customerId)
        => Query.Where(c => c.CustomerId == customerId);
}
