using System.Text.RegularExpressions;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Exceptions;

namespace TE4IT.Domain.ValueObjects;

/// <summary>
/// Email değer nesnesi - type safety ve validation sağlar
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public string Value { get; }

    private Email(string value)
    {
        Value = ValidateEmail(value);
    }

    public static implicit operator string(Email email) => email.Value;

    public static Email Create(string value) => new Email(value);

    public static bool TryCreate(string value, out Email email)
    {
        try
        {
            email = new Email(value);
            return true;
        }
        catch
        {
            email = null!;
            return false;
        }
    }

    private static string ValidateEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException(DomainConstants.RequiredFieldMessage);

        if (value.Length > DomainConstants.MaxEmailLength)
            throw new InvalidEmailException($"E-posta adresi {DomainConstants.MaxEmailLength} karakterden uzun olamaz.");

        if (!EmailRegex.IsMatch(value))
            throw new InvalidEmailException(DomainConstants.InvalidEmailMessage);

        return value.ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
