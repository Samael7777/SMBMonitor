namespace SmbMonitor.Base.Interfaces.Tools;

public interface IMountedDiskLettersResolver
{
    /// <summary>
    /// Gets drive letters, used by mounted disks
    /// </summary>
    /// <returns>IEnumerable of letters in upper case</returns>
    IEnumerable<char> GetMountedDiskLetters();
}
