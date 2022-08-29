using System.Text;

namespace SmbMonitorLib.Wifi;


public class WifiNetworkIdentifier : IEquatable<WifiNetworkIdentifier>
{
    private readonly byte[] _rawBytes;

    private static readonly Lazy<Encoding> encoding = new(() =>
        Encoding.GetEncoding(65001, // UTF-8 code page
            EncoderFallback.ReplacementFallback,
            DecoderFallback.ExceptionFallback));

    public WifiNetworkIdentifier(byte[] ssid)
    {
        _rawBytes = new byte[ssid.Length];
        if (ssid.Length > 0)
            Array.Copy(ssid, _rawBytes, ssid.Length);
    }

    public byte[] ToBytes()
    {
        return _rawBytes;
    }

    public override string ToString()
    {
        if (_rawBytes.Length <= 0) return string.Empty;

        string value;
        try
        {
            value = encoding.Value.GetString(ToBytes());
        }
        catch (DecoderFallbackException)
        {
            value = string.Empty;
        }
        return value;
    }

    public bool Equals(WifiNetworkIdentifier? other)
    {
        return other != null && _rawBytes.SequenceEqual(other._rawBytes);
    }
    public override bool Equals(object? obj)
    {
        return Equals(obj as WifiNetworkIdentifier);
    }

    public override int GetHashCode()
    {
        return _rawBytes.GetHashCode();
    }
}