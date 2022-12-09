using System.Text;

namespace NativeApi.Wifi.Base;

public readonly struct WifiSSID : IEquatable<WifiSSID>
{
    public static readonly WifiSSID Empty = new(new byte[] { 0 });

    public byte[] SSIDBytes { get; }

    public WifiSSID(byte[] ssid)
    {
        if (ssid.Length <= 0)
            throw new ArgumentException("SSID must be not empty array.");

        SSIDBytes = new byte[ssid.Length];
        Array.Copy(ssid, SSIDBytes, ssid.Length);
    }

    public WifiSSID(string ssid)
    {
        if (ssid == string.Empty)
            throw new ArgumentException("SSID must be not empty string.");

        SSIDBytes = Encoding.UTF8.GetBytes(ssid);
    }

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
        var data = wifiSSID.SSIDBytes;
        var result = "";

        if (data.Length <= 0) return string.Empty;

        const int utf8Codepage = 65001;
        var encoding = new Lazy<Encoding>(() =>
            Encoding.GetEncoding(utf8Codepage, EncoderFallback.ReplacementFallback,
                DecoderFallback.ExceptionFallback));
        try
        {
            result = encoding.Value.GetString(data);
        }
        catch (DecoderFallbackException)
        { }

        return result;
    }
}