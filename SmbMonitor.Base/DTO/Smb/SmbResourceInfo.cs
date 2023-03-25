namespace SmbMonitor.Base.DTO.Smb;

public readonly record struct SmbResourceInfo
(
    Uri RemoteName,
    string MappedDisk = "",
    string Comments = "",
    string Provider = ""
)
{
    public bool IsEmpty => RemoteName == null;
}