namespace ProfileAPI.ApplicationCore.Common.Domain;

public abstract class DomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        DateOccurred = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public DateTimeOffset DateOccurred { get; private set; }
}
