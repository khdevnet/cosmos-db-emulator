using Ardalis.Specification;

namespace ProfileAPI.ApplicationCore.Common.Domain;

public interface IRepository<T> : IRepositoryBase<T>
    where T : class, IAggregateRoot
{
}
