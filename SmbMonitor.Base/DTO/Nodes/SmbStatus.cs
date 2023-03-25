namespace SmbMonitor.Base.DTO.Nodes;

public enum SmbStatus
{
    Unknown = 0,
    Disconnecting,
    Disconnected,
    Connected,
    Connecting,
    PartiallyConnected,
    NoSharesAvailable,
    Updating
}