using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Exceptions.Tasks;

namespace TE4IT.Domain.ValueObjects;

/// <summary>
/// Öncelik değer nesnesi - görev ve proje önceliklerini temsil eder
/// </summary>
public sealed class Priority : ValueObject
{
    public const int MinValue = 1;
    public const int MaxValue = 5;

    public int Value { get; }

    private Priority(int value)
    {
        Value = ValidatePriority(value);
    }

    public static implicit operator int(Priority priority) => priority.Value;

    public static Priority Create(int value) => new Priority(value);

    public static Priority Low => new Priority(1);
    public static Priority Medium => new Priority(3);
    public static Priority High => new Priority(5);

    public static Priority[] GetAllPriorities()
    {
        return Enumerable.Range(MinValue, MaxValue - MinValue + 1)
            .Select(Create)
            .ToArray();
    }

    public string GetDisplayName()
    {
        return Value switch
        {
            1 => "Çok Düşük",
            2 => "Düşük",
            3 => "Orta",
            4 => "Yüksek",
            5 => "Çok Yüksek",
            _ => "Bilinmeyen"
        };
    }

    private static int ValidatePriority(int value)
    {
        if (value < MinValue || value > MaxValue)
            throw new InvalidPriorityException($"Öncelik değeri {MinValue} ile {MaxValue} arasında olmalıdır.");

        return value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => GetDisplayName();
}
