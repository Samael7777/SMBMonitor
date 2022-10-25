using System.Text;

namespace WifiAPI.Base;

public readonly struct WifiSSID : IEquatable<WifiSSID>
{
    public WifiSSID(byte[] ssid)
    {
        SSID = new byte[ssid.Length];
        if (ssid.Length > 0)
            Array.Copy(ssid, SSID, ssid.Length);
    }

    public WifiSSID(string ssid)
    {
        SSID = Encoding.UTF8.GetBytes(ssid);
    }

    public byte[] SSID { get; }

    public override string ToString()
    {
        var result = string.Empty;

        try
        {
            result = Decode(SSID);
        }
        catch (DecoderFallbackException)
        {
        }

        return result;
    }

    private static string Decode(byte[] data)
    {
        if (data.Length <= 0) return string.Empty;
        const int utf8Codepage = 65001;
        var encoding = new Lazy<Encoding>(() =>
            Encoding.GetEncoding(utf8Codepage, EncoderFallback.ReplacementFallback,
                DecoderFallback.ExceptionFallback));

        return encoding.Value.GetString(data);
    }

    #region Equals

    public bool Equals(WifiSSID other)
    {
        return SSID.SequenceEqual(other.SSID);
    }

    public override bool Equals(object? obj)
    {
        return obj is WifiSSID identifier && Equals(identifier);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.AddBytes(SSID);
        return hash.ToHashCode();
    }

    public static bool operator ==(WifiSSID left, WifiSSID right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(WifiSSID left, WifiSSID right)
    {
        return !(left == right);
    }

    #endregion
}