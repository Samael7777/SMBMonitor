using NativeApi.NetworkUtils;
using NativeApi.Smb.Base;

namespace SmbMonitorLib.Interfaces;

public interface ISmbService : IBaseService
{
    void ConnectShareWithLettersReservation(SmbResourceInfo share, Credentials credentials);
    void DisconnectAllShares(Host server);
    void DisconnectShareAndFreeLetter(SmbResourceInfo share);
    List<Host> GetConnectedServers();
    List<SmbResourceInfo> GetServerConnectedShares(Host server);
    List<SmbResourceInfo> GetServerDisconnectedShares(Host server, Credentials credentials);
    List<SmbResourceInfo> GetServerShares(Host server, Credentials credentials);
    List<SmbResourceInfo> GetConnectedShares();
}