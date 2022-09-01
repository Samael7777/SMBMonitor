// ReSharper disable NonReadonlyMemberInGetHashCode
namespace SmbMonitorLib;

/// <summary>
///     Класс параметров мониторинга
/// </summary>
public class Timings : IEquatable<Timings>
{
    /// <summary>
    ///     Интервал опроса порта, мс
    /// </summary>
    public int Interval { get; set; }
    /// <summary>
    ///     Таймаут опроса порта, мс
    /// </summary>
    public int Timeout { get; set; }

    public bool Equals(Timings? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Interval == other.Interval && Timeout == other.Timeout;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Timings)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Interval, Timeout);
    }
}
