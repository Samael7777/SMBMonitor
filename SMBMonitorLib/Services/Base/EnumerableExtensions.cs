using System.Collections;

namespace SmbMonitorLib.Services.Base;

internal static class EnumerableExtensions
{
    public static List<T>ToList<T>(this IEnumerable enumerable)
    {
        var list = new List<T>();
        foreach (var obj in enumerable)
        {
            if (obj is T item)
                list.Add(item);
            else
                throw new ArgumentException($"Can't cast item to {typeof(T)}");
        }
        return list;
    }
}
