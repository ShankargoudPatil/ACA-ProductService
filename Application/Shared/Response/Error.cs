using System.Text.Json.Serialization;
namespace Application.Shared.Response;
public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorTypes.Failure);

    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.", ErrorTypes.Failure);
    public string Code { get; } = string.Empty;
    public string? Description { get; }
    [JsonIgnore] public ErrorTypes ErrorTypeEnum { get; }
    public string ErrorType { get; } = string.Empty;

    public Error(string code, string description, ErrorTypes errorType)
    {
        Code = code;
        Description = description;
        ErrorTypeEnum = errorType;
        ErrorType = errorType.ToString();
    }
    public static Error NotFound(string code, string description) =>
            new(code, description, ErrorTypes.NotFound);
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorTypes.Validation);
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorTypes.Conflict);
    public static Error BadRequest(string code, string description) =>
     new(code, description, ErrorTypes.BadRequest);
    public static Error Forbidden(string code, string description) =>
     new(code, description, ErrorTypes.Forbidden);

    public static Error Failure(string code, string description) =>
    new Error(code, description, ErrorTypes.Failure);
}

public enum ErrorTypes
{
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    BadRequest = 4,
    Forbidden = 5,
    Conflict = 6
}


