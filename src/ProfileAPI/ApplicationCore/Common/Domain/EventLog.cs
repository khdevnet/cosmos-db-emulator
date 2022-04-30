namespace ProfileAPI.ApplicationCore.Common.Domain;

public class EventLog
{
    //public EventLog(string id, string type, string data)
    //{
    //    Id = Guard.Against.Default(id);
    //    Type = Guard.Against.NullOrEmpty(type);
    //    Data = Guard.Against.NullOrEmpty(data);
    //}

    //public EventLog(DomainEvent dEvent)
    //{
    //    Guard.Against.Null(dEvent);

    //    Id = Guard.Against.Default(dEvent.Id).ToString();
    //    Type = dEvent.GetType().FullName;
    //    Data = dEvent.ToJson();
    //}

    public Guid Id { get; set; }

    public string? Type { get; set; }

    public string? TopicName { get; set; }

    public string? Data { get; set; }
}
