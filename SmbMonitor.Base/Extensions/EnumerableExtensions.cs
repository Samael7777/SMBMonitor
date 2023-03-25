namespace SmbMonitor.Base.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        using var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            action?.Invoke(enumerator.Current);
        }
    }

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        using var enumerator = enumerable.GetEnumerator();
        return !enumerator.MoveNext();
    }
}