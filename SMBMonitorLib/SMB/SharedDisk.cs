using SmbMonitorLib.SMB.Win32;

namespace SmbMonitorLib.SMB;

/// <summary>
///     Сетевой диск
/// </summary>
public class SharedDisk : IEquatable<SharedDisk>
{
    /// <summary>
    ///     Имя локального диска
    /// </summary>
    public string LocalName { get; set; }

    /// <summary>
    ///     Имя сетевого ресурса
    /// </summary>
    public string RemoteName { get; }

    /// <summary>
    ///     Комментарии
    /// </summary>
    public string Comments { get; }

    /// <summary>
    ///     Провайдер
    /// </summary>
    public string Provider { get; }

    /// <summary>
    ///     Имя сервера
    /// </summary>
    public string RootPath => GetServerNameFromRemotePath();

    public SharedDisk()
    {
        Provider = string.Empty;
        Comments = string.Empty;
        LocalName = string.Empty;
        RemoteName = string.Empty;
    }
    internal SharedDisk(NetResource? value)
    {
        Provider = value?.Provider ?? string.Empty;
        Comments = value?.Comments ?? string.Empty;
        LocalName = value?.LocalName ?? string.Empty;
        RemoteName = value?.RemoteName ?? string.Empty;
    }

    public bool Equals(SharedDisk? other)
    {
        if (other is null) return false;
        
        return ReferenceEquals(this, other) 
               || RemoteName.Equals(other.RemoteName,StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return obj.GetType() == GetType() && Equals((SharedDisk)obj);
    }

    public override int GetHashCode()
    {
        return RemoteName.GetHashCode();
    }

    private string GetServerNameFromRemotePath()
    {
        if (RemoteName.Equals(string.Empty))
            return string.Empty;

        var serverName = "\\\\" + RemoteName.Split('\\')[2];
        return serverName;
    }
}