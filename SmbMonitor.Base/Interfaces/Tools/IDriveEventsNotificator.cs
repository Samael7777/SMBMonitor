namespace SmbMonitor.Base.Interfaces.Tools;

public delegate void DriveStateHandler(string driveLetter, DriveType driveType, DriveState state);

public enum DriveState
{
    Unknown,
    Arrival,
    RemoveComplete
}

public interface IDriveEventsNotificator : INotificator
{
    event DriveStateHandler? OnDriveStateChange;
}