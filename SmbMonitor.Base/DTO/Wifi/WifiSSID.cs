using System.Text;

namespace SmbMonitor.Base.DTO.Wifi;

public readonly struct WifiSSID : IEquatable<WifiSSID>
{
    public static readonly WifiSSID Empty = new(true);

    public byte[] SSIDBytes { get; }

    private WifiSSID(bool isEmpty)
    {
        IsEmpty = isEmpty;
        SSIDBytes = Array.Empty<byte>();
    }

    public WifiSSID(byte[] ssid) : this(false)
    {
        if (ssid.Length <= 0)
            throw new ArgumentNullException(nameof(ssid));

        SSIDBytes = new byte[ssid.Length];
        Array.Copy(ssid, SSIDBytes, ssid.Length);
    }

    public WifiSSID(string ssid) : this(false)
    {
        if (ssid == string.Empty)
            throw new ArgumentNullException(nameof(ssid));

        SSIDBytes = Encoding.UTF8.GetBytes(ssid);
    }

    public bool IsEmpty { get; }

    public override string ToString()
    {
        return SsidHelper.GetString(this);
    }

    #region Equals

    public bool Equals(WifiSSID other)
    {
        return SSIDBytes.SequenceEqual(other.SSIDBytes);
    }

    public override bool Equals(object? obj)
    {
        return obj is WifiSSID other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.AddBytes(SSIDBytes);
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

internal static class SsidHelper
{
    public static string GetString(WifiSSID wifiSSID)
    {
        const int utf8Codepage = 65001;
        
        var data = wifiSSID.SSIDBytes;
        var result = string.Empty;

        if (data.Length <= 0) return result;
        
        var encoding = new Lazy<Encoding>(() =>
            Encoding.GetEncoding(utf8Codepage, EncoderFallback.ReplacementFallback,
                DecoderFallback.ExceptionFallback));
        try
        {
            result = encoding.Value.GetString(data);
        }
        catch (DecoderFallbackException)
        {
            result = string.Empty;
        }

        return result;
    }
}