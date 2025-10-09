using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Exceptions;

namespace TE4IT.Domain.ValueObjects;

/// <summary>
/// Süre değer nesnesi - görev ve proje sürelerini temsil eder
/// </summary>
public sealed class Duration : ValueObject
{
    public TimeSpan Value { get; }

    private Duration(TimeSpan value)
    {
        Value = ValidateDuration(value);
    }

    public static implicit operator TimeSpan(Duration duration) => duration.Value;

    public static Duration Create(TimeSpan value) => new Duration(value);

    public static Duration Create(int hours) => new Duration(TimeSpan.FromHours(hours));

    public static Duration Create(int days, int hours = 0) => new Duration(TimeSpan.FromDays(days).Add(TimeSpan.FromHours(hours)));

    public static Duration Zero => new Duration(TimeSpan.Zero);

    public int TotalHours => (int)Value.TotalHours;
    public int TotalDays => (int)Value.TotalDays;
    public int Hours => Value.Hours;
    public int Days => Value.Days;

    public string GetDisplayString()
    {
        if (Value.TotalDays >= 1)
        {
            var days = (int)Value.TotalDays;
            var hours = Value.Hours;

            if (hours > 0)
                return $"{days} gün {hours} saat";
            return $"{days} gün";
        }

        if (Value.TotalHours >= 1)
        {
            var hours = (int)Value.TotalHours;
            var minutes = Value.Minutes;

            if (minutes > 0)
                return $"{hours} saat {minutes} dakika";
            return $"{hours} saat";
        }

        return $"{Value.Minutes} dakika";
    }

    public string GetShortDisplayString()
    {
        if (Value.TotalDays >= 1)
            return $"{(int)Value.TotalDays}d";

        if (Value.TotalHours >= 1)
            return $"{(int)Value.TotalHours}h";

        return $"{Value.Minutes}m";
    }

    public bool IsZero => Value == TimeSpan.Zero;
    public bool IsPositive => Value > TimeSpan.Zero;

    public Duration Add(Duration other)
    {
        return new Duration(Value.Add(other.Value));
    }

    public Duration Subtract(Duration other)
    {
        return new Duration(Value.Subtract(other.Value));
    }

    private static TimeSpan ValidateDuration(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
            throw new InvalidDurationException("Süre negatif olamaz.");

        var maxDuration = TimeSpan.FromDays(DomainConstants.MaxTaskDurationDays);
        if (value > maxDuration)
            throw new InvalidDurationException($"Süre {DomainConstants.MaxTaskDurationDays} günden uzun olamaz.");

        return value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => GetDisplayString();
}
