using SmbAPI.Win32.DTO;

namespace SmbAPI.Base;

public struct SmbResourceInfo : IEquatable<SmbResourceInfo>
{
    internal SmbResourceInfo(NetResource value)
    {
        if (value.RemoteName is null)
            throw new ArgumentNullException(nameof(value), "RemoteName must be not null!");

        Provider = value.Provider ?? string.Empty;
        Comments = value.Comments ?? string.Empty;
        LocalName = value.LocalName ?? string.Empty;
        RemoteName = new SmbPath(value.RemoteName);
    }

    public SmbResourceInfo(string remoteName)
    {
        if (remoteName == string.Empty)
            throw new ArgumentNullException(nameof(remoteName));

        RemoteName = new SmbPath(remoteName);
    }

    public SmbPath RemoteName { get; }

    public string LocalName { get; set; } = "";

    public string Comments { get; set; } = "";

    public string Provider { get; set; } = "";

    #region Equals

    public bool Equals(SmbResourceInfo other)
    {
        return RemoteName.Equals(other.RemoteName);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        return obj.GetType() == GetType() && Equals((SmbResourceInfo)obj);
    }

    public override int GetHashCode()
    {
        return RemoteName.GetHashCode();
    }

    public static bool operator ==(SmbResourceInfo left, SmbResourceInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SmbResourceInfo left, SmbResourceInfo right)
    {
        return !(left == right);
    }

    #endregion
}