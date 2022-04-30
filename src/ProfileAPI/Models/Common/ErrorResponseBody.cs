using ProfileAPI.ApplicationCore.Common;

namespace ProfileAPI.Common.Models;

public class ErrorResponseBody
{
    protected ErrorResponseBody(Enum @enum, object data, object[]? args, string exceptionMessage)
    {
        Code = Convert.ToInt32(@enum);
        Key = Enum.GetName(@enum.GetType(), @enum) ?? string.Empty;
        Message = @enum.GetEnumDisplayNameValueOrNull();

        if (args != null)
        {
            Message = string.Format(Message!, args);
        }

        Data = data;
        ExceptionMessage = exceptionMessage;
    }

    public int Code { get; }

    public string Key { get; }

    public string? Message { get; }

    public string? ExceptionMessage { get; }

    public object? Data { get; }

    public static ErrorResponseBody Create<TData>(Enum @enum, TData? data = default, object[]? args = default, string? exceptionMessage = default)
        => new(@enum, data, args, exceptionMessage);

    public static ErrorResponseBody Create(Enum @enum, string? exceptionMessage = default)
        => Create<object>(@enum, default, null, exceptionMessage);
}
