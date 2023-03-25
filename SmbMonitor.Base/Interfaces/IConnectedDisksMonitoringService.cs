using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Base.Interfaces;

public interface IConnectedDisksMonitoringService
{
    void OnDriveStateChange(string driveLetter, DriveType driveType, DriveState state);
}
