using Ardalis.Specification.EntityFrameworkCore;
using ProfileAPI.ApplicationCore.Common.Domain;

namespace ProfileAPI.Infrastructure.Database;

public class EfRepository<T> : RepositoryBase<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public EfRepository(NotifyMeDbContext dbContext)
        : base(dbContext)
    {
    }
}
