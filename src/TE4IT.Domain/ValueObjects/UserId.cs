using TE4IT.Domain.Entities.Common;

namespace TE4IT.Domain.ValueObjects;

public sealed class UserId : ValueObject
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("UserId boş olamaz", nameof(value));
        Value = value;
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid value) => new UserId(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}


