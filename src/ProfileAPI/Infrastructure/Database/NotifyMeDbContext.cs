using ProfileAPI.ApplicationCore.Common;
using ProfileAPI.ApplicationCore.Common.Domain;
using ProfileAPI.ApplicationCore.Domain;
using ProfileAPI.Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProfileAPI.Infrastructure.Database;

public class NotifyMeDbContext : DbContext
{
    //private readonly IBus _messageBus;

    public NotifyMeDbContext(
        DbContextOptions<NotifyMeDbContext> options)
     : base(options)
    {
    }

    public virtual DbSet<Subscription> Subscriptions => Set<Subscription>();

    public virtual DbSet<EventLog> EventLogs => Set<EventLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BuildCustomers(modelBuilder);

        BuildEventLogs(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Select(e => e.Entity as BaseEntity)
            .Where(e => e?.Events != null && e.Events.Any())
            .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity!.Events.ToArray();
            entity.Events.Clear();

            var eventsLog = events.Select(ev => new EventLog
            {
                Id = ev.Id,
                TopicName = ev.GetType().Name.ToLower(),
                Data = ev.ToJson(),
                Type = ev.GetType().FullName,
            }).ToArray();

            AddRange(eventsLog);
        }

        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result;
    }

    public override int SaveChanges()
        => SaveChangesAsync().GetAwaiter().GetResult();

    private static void BuildCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>().ToContainer("Subscriptions");

        modelBuilder.Entity<Subscription>()
            .HasPartitionKey(nameof(Subscription.CustomerId));

        modelBuilder.Entity<Subscription>()
            .HasNoDiscriminator();

        modelBuilder.Entity<Subscription>()
            .HasKey(nameof(Subscription.CustomerId));

        modelBuilder.Entity<Subscription>()
            .Property(nameof(Subscription.CustomerId))
            .HasConversion(new GuidToStringConverter());

        modelBuilder.Entity<Subscription>()
          .Property(nameof(Subscription.VehicleIds))
          .HasConversion(
            new GuidListValueConverter(),
            new ValueComparer<IList<Guid>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
    }

    private static void BuildEventLogs(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventLog>().ToContainer("EventLogs");

        modelBuilder.Entity<EventLog>()
           .HasNoDiscriminator();

        modelBuilder.Entity<EventLog>()
            .HasKey(nameof(EventLog.Id));

        modelBuilder.Entity<EventLog>()
            .Property(nameof(EventLog.Id))
            .HasConversion(new GuidToStringConverter());

        modelBuilder.Entity<EventLog>()
            .HasPartitionKey(nameof(EventLog.Type));
    }
}
