using System.Text.RegularExpressions;

namespace DddValueObjects.Api.Domain.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email address cannot be empty.");
        }

        var trimmedValue = value.Trim();

        if (!EmailRegex.IsMatch(trimmedValue))
        {
            throw new ArgumentException($"Invalid email format: '{value}'");
        }

        Value = trimmedValue;
    }

    public override string ToString() => Value;
}
