namespace NativeApi.Smb.Base;

public readonly record struct SmbResourceInfo(SmbPath RemoteName, string LocalName = "",
    string Comments = "", string Provider = "")
{
    public bool Equals(SmbResourceInfo other)
    {
        return RemoteName.Equals(other.RemoteName);
    }

    public override int GetHashCode()
    {
        return RemoteName.GetHashCode();
    }
}