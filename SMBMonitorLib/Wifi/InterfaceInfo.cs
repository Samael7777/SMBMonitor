using SmbMonitorLib.Wifi.Win32;

namespace SmbMonitorLib.Wifi;

/// <summary>
///     Wireless interface information
/// </summary>
public class InterfaceInfo
{
    private InterfaceInfo()
    {
        Id = Guid.Empty;
        Description = string.Empty;
        State = InterfaceState.NotReady;
    }

    internal InterfaceInfo(WlanInterfaceInfo info)
    {
        Id = info.InterfaceGuid;
        Description = info.InterfaceDescription;
        State = (InterfaceState)info.isState;
    }

    /// <summary>
    ///     Interface ID
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Interface description
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Interface state
    /// </summary>
    public InterfaceState State { get; }

    /// <summary>
    ///     Returns an empty class
    /// </summary>
    /// <returns>An empty InterfaceInfo</returns>
    public static InterfaceInfo Empty => new();

    /// <summary>
    ///     Returns true for empty class
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return Id != Guid.Empty
               || Description != string.Empty
               || State != InterfaceState.NotReady;
    }
}