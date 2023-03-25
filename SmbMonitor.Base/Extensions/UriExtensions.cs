namespace SmbMonitor.Base.Extensions;

public static class UriExtensions
{
    public static string GetSmbConnectionString(this Uri uri)
    {
        if (uri.Scheme != "file")
            throw new ArgumentException("Unsupported URI scheme.");

        return $@"\\{uri.DnsSafeHost}{uri.AbsolutePath.Replace('/', '\\')}";
    }
}