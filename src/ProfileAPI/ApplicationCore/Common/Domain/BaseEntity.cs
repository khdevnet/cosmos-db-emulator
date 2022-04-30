namespace ProfileAPI.ApplicationCore.Common.Domain;

public abstract class BaseEntity
{
    public List<DomainEvent> Events = new();
}
