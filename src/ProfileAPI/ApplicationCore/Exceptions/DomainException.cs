using ProfileAPI.Common.Models;

namespace ProfileAPI.ApplicationCore.Exceptions;

public class DomainException : Exception
{
    public DomainException(ErrorCode code, object? data = null, object[]? args = null)
    {
        Code = code;
        Data = data;
        Args = args;
    }

    public ErrorCode Code { get; }

    public object[]? Args { get; }

    public object? Data { get; }
}
