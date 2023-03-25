using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Tools;

public class MountedDiskLettersResolver : IMountedDiskLettersResolver
{
    public IEnumerable<char> GetMountedDiskLetters()
    {
        var drives = DriveInfo.GetDrives();
        return drives.Select(di => char.ToUpper(di.Name[0]));
    }
}
