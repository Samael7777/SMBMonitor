using System.Net;
using System.Net.Sockets;
using NetworkUtils;
using Newtonsoft.Json;

namespace NativeApi.NetworkUtils;

[JsonObject(MemberSerialization.OptIn)]
public readonly struct Host : IEquatable<Host>
{
    public static readonly Host Empty = new();

    [JsonProperty]
    private readonly IPAddress _address = IPAddress.None;
    
    [JsonProperty]
    private readonly string _name = string.Empty;

    public string Description { get; }

    public IPAddress IPAddress => IsAddressIncorrect(_address)
        ? string.IsNullOrEmpty(_name) ? IPAddress.None : IPv4Resolver.GetAddress(_name)
        : _address;

    public string DomainName => string.IsNullOrEmpty(_name)
        ? _address.Equals(IPAddress.None) ? "" : _address.GetDomainNameOrIP()
        : _name;

    public Host()
    {
        _address = IPAddress.None;
        _name = string.Empty;
        Description = string.Empty;
    }

    public Host(IPAddress address)
    {
        if (IsAddressIncorrect(address))
            throw new ArgumentException("Address must be correct IPv4 single host address.");

        _address = address;
        Description = address.ToString();
    }

    public Host(string name)
    {
        if (name.Length == 0)
            throw new ArgumentException("Name must be correct domain name.");
        _name = name;
        Description = name;
    }

    private static bool IsAddressIncorrect(IPAddress? address)
    {
        if (address is null) return false;
        return address.AddressFamily != AddressFamily.InterNetwork
               || address.Equals(IPAddress.None)
               || address.Equals(IPAddress.Any)
               || address.Equals(IPAddress.Broadcast);
    }

    #region Equals

    public bool Equals(Host other)
    {
        return !_address.Equals(IPAddress.None)
            ? _address.Equals(other._address)
            : string.Equals(_name, other._name, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is Host host && Equals(host);
    }

    public override int GetHashCode()
    {
        return !string.IsNullOrEmpty(_name)
            ? _name.GetHashCode()
            : _address.GetHashCode();
    }

    public static bool operator ==(Host left, Host right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Host left, Host right)
    {
        return !(left == right);
    }

    #endregion
}