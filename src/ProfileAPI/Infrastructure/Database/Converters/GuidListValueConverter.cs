using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProfileAPI.Infrastructure.Database.Converters;

public class GuidListValueConverter : ValueConverter<IList<Guid>, IList<string>>
{
    public GuidListValueConverter()
        : base(
            v => v.Select(c => c.ToString()).ToList(),
            v => v.Select(c => Guid.Parse(c)).ToList())
    {
    }
}
