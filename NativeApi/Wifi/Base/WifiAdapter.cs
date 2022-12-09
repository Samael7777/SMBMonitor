using NativeApi.Wifi.Win32.DTO;

namespace NativeApi.Wifi.Base;

public class WifiAdapter
{
    internal WifiAdapter(WlanInterfaceInfo info)
    {
        Id = info.InterfaceGuid;
        Description = info.InterfaceDescription;
        State = (InterfaceState)info.isState;
    }

    public Guid Id { get; }

    public string Description { get; }

    public InterfaceState State { get; }
}