using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using NativeApi.NetworkUtils;
using NetworkUtils;

namespace NativeApi.Smb.Base;

public class SmbPath : IEquatable<SmbPath>
{
    private readonly string _serverName;

    public SmbPath(string remotePath)
    {
        CheckRemotePath(remotePath);
        _serverName = GetServerNameFromPath(remotePath);
        ParseRelativePath(remotePath);
    }

    public string FullPath => GetFullPath(ServerName);

    public string FullPathWithIP => GetFullPath(GetServerAddress().ToString());

    public string RelativePath { get; private set; }

    public string ServerName => GetServerName();

    public IPAddress ServerAddress => GetServerAddress();

    public override string ToString()
    {
        return $"\\\\{_serverName}\\{RelativePath}";
    }

    private static void CheckRemotePath(string? remotePath)
    {
        if (string.IsNullOrEmpty(remotePath))
            throw new ArgumentNullException(nameof(remotePath), "Remote path must be not empty!");

        if (!remotePath.StartsWith(@"\\"))
            throw new ArgumentException(@"Remote path must start with \\");
    }

    [MemberNotNull(nameof(RelativePath))]
    private void ParseRelativePath(string remotePath)
    {
        var trimmedPath = remotePath.TrimStart('\\');
        var firstSlashIndex = trimmedPath.IndexOf('\\');

        RelativePath = firstSlashIndex < 0
            ? ""
            : trimmedPath[(firstSlashIndex + 1)..];
    }

    private string GetServerName()
    {
        return IPAddress.TryParse(_serverName, out var address)
            ? address.GetDomainNameOrIP()
            : _serverName;
    }

    private IPAddress GetServerAddress()
    {
        return IPAddress.TryParse(_serverName, out var address)
            ? address
            : IPv4Resolver.GetAddress(ServerName);
    }

    private string GetFullPath(string server)
    {
        var result = new StringBuilder();
        result.Append(@"\\");
        result.Append(server);
        result.Append('\\');
        result.Append(RelativePath);

        return result.ToString();
    }

    private static string GetServerNameFromPath(string remotePath)
    {
        return remotePath.Split('\\')[2];
    }

    #region Equals

    public bool Equals(SmbPath? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (!RelativePath.Equals(other.RelativePath, StringComparison.OrdinalIgnoreCase)) return false;
        if (_serverName.Equals(other._serverName, StringComparison.OrdinalIgnoreCase)) return true;
        if (ServerAddress.Equals(other.ServerAddress)) return true;
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType()
               && Equals((SmbPath)obj);
    }

    public override int GetHashCode()
    {
        return _serverName.GetHashCode();
    }

    #endregion
}