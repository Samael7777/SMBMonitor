using SmbMonitor.Base.DTO.Smb;

namespace SmbMonitor.Base.Comparers;


/// <summary>
/// Compares shares with obviously same hosts
/// </summary>
public class HostSharesComparer : IEqualityComparer<SmbResourceInfo>
{
    public bool Equals(SmbResourceInfo x, SmbResourceInfo y)
    {
        return string.Equals(x.RemoteName.AbsolutePath, y.RemoteName.AbsolutePath, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(SmbResourceInfo obj)
    {
        return obj.RemoteName.GetHashCode();
    }
}
