using SmbMonitor.Base.DTO.Smb;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface ISmbClient
{
    void ConnectNetworkDisk(Uri path, char diskLetter, string user, string password);
    void DisconnectNetworkDisk(string disk);
    List<SmbResourceInfo> GetConnectedShares();
    List<SmbResourceInfo> GetRemoteShares(string server, string user, string password);
}