using Base;
using NetworkUtils;
using SmbAPI.Base;

namespace SmbMonitorLib.Services.Interfaces;

public interface ISmbManager
{
    void ConnectSharesWithLettersReservation(IEnumerable<SmbResourceInfo> shares, Credentials credentials);
    void DisconnectAllShares(Host server);
    void DisconnectShareAndFreeLetter(SmbResourceInfo share);
    IEnumerable<Host> GetConnectedServers();
    IEnumerable<SmbResourceInfo> GetServerConnectedShares(Host server);
    IEnumerable<SmbResourceInfo> GetServerDisconnectedShares(Host server, Credentials credentials);
    IEnumerable<SmbResourceInfo> GetServerShares(Host server, Credentials credentials);
}