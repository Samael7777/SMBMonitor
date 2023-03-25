// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace SmbMonitor.Base.DTO.Wifi;

public class ConnectionFlags
{
    /// <summary>
    ///     ANQP is supported
    /// </summary>
    public bool AnqpSupported;

    /// <summary>
    ///     This network failed to connect
    /// </summary>
    public bool AutoConnectFailed;

    /// <summary>
    ///     This network is currently connected
    /// </summary>
    public bool Connected;

    /// <summary>
    ///     The profile is the active console user's per user profile
    /// </summary>
    public bool ConsoleUserProfile;

    /// <summary>
    ///     There is a profile for this network
    /// </summary>
    public bool HasProfile;

    /// <summary>
    ///     Domain network
    /// </summary>
    public bool Hotspot2Domain;

    /// <summary>
    ///     Hotspot2 is enabled
    /// </summary>
    public bool Hotspot2Enabled;

    /// <summary>
    ///     Roaming network
    /// </summary>
    public bool Hotspot2Roaming;

    /// <summary>
    ///     Interworking is supported
    /// </summary>
    public bool InterworkingSupported;

    public static ConnectionFlags FromBitMask(uint bitMask) =>
        new() 
        {
            Connected = (bitMask & 0x00000001) > 0,
            HasProfile = (bitMask & 0x00000002) > 0,
            ConsoleUserProfile = (bitMask & 0x00000004) > 0,
            InterworkingSupported = (bitMask & 0x00000008) > 0,
            Hotspot2Enabled = (bitMask & 0x00000010) > 0,
            AnqpSupported = (bitMask & 0x00000020) > 0,
            Hotspot2Domain = (bitMask & 0x00000040) > 0,
            Hotspot2Roaming = (bitMask & 0x00000080) > 0,
            AutoConnectFailed = (bitMask & 0x00000100) > 0
        };
}