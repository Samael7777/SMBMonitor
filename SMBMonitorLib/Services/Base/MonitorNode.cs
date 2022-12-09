using NativeApi.NetworkUtils;
using NativeApi.Wifi.Base;

namespace SmbMonitorLib.Services.Base;

public enum NodeType
{
    Unknown, AP,
    Host,
    ExternalHost
}
public class MonitorNode : IEquatable<MonitorNode>
{
    public NodeType Type { get; init; } = NodeType.Unknown;
    public string Description { get; init; } = string.Empty;
    public Credentials Credentials { get; init; } = new();
    public WifiSSID WifiSSID { get; init; } = WifiSSID.Empty;
    public Host Host { get; set; } = Host.Empty;
    
    #region Equals

    public bool Equals(MonitorNode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Type != other.Type) return false;
        return Type switch
        {
            NodeType.Host => Host.Equals(other.Host),
            NodeType.ExternalHost => Host.Equals(other.Host),
            NodeType.AP => WifiSSID.Equals(other.WifiSSID),
            _ => GetHashCode() == other.GetHashCode()
        };
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((MonitorNode)obj);
    }

    public override int GetHashCode()
    {
        return Type switch
        {
            NodeType.AP => HashCode.Combine((int)Type, WifiSSID),
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            NodeType.Host => HashCode.Combine((int)Type, Host),
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            NodeType.ExternalHost => HashCode.Combine((int)Type, Host),
            _ => GetHashCode()
        };
    }

    #endregion
}