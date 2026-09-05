namespace ResultPattern.Api.Common;

/// <summary>
/// Represents a domain error with a code and description.
/// </summary>
public sealed record Error(string Code, string Description)
{
    /// <summary>No error — represents a successful result.</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Generic null-value error.</summary>
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided.");

    /// <summary>Creates a Not Found error.</summary>
    public static Error NotFound(string resource, object id) =>
        new($"{resource}.NotFound", $"{resource} with id '{id}' was not found.");

    /// <summary>Creates a Validation error.</summary>
    public static Error Validation(string field, string message) =>
        new($"Validation.{field}", message);

    /// <summary>Creates a Conflict error.</summary>
    public static Error Conflict(string resource, string reason) =>
        new($"{resource}.Conflict", reason);

    /// <summary>Creates an Unauthorized error.</summary>
    public static Error Unauthorized(string reason = "Unauthorized access.") =>
        new("Auth.Unauthorized", reason);
}
