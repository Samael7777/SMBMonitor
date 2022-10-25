using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using NetworkUtils;

namespace SmbAPI.Base;

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

    public string FullPathWithIP => GetFullPath(ServerAddress.ToString());

    public string RelativePath { get; private set; }

    public string ServerName => GetServerName();

    public IPAddress ServerAddress => GetServerAddress();


    private static void CheckRemotePath(string remotePath)
    {
        if (remotePath == string.Empty)
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
            ? new DomainNameResolver(address).GetDomainNameOrIP()
            : _serverName;
    }

    private IPAddress GetServerAddress()
    {
        return IPAddress.TryParse(_serverName, out var address)
            ? address
            : new IPv4Resolver(ServerName).GetAddress();
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

        return RelativePath.Equals(other.RelativePath, StringComparison.OrdinalIgnoreCase)
               && ServerName.Equals(other.ServerName, StringComparison.OrdinalIgnoreCase);
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