namespace SmbMonitor.Base.Interfaces.Tools;

public delegate void AdapterStateHandler(Guid adapter, AdapterState state);

public enum AdapterState
{
    Unknown,
    Connected,
    Disconnected
}

public interface IWifiEventsNotificator : INotificator
{
    event AdapterStateHandler? OnAdapterStateChanged;
}