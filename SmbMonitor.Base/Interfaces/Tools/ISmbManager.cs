using SmbMonitor.Base.DTO.Smb;

namespace SmbMonitor.Base.Interfaces.Tools;

public interface ISmbManager
{
    bool TryConnectSharesWithAutoLetterReservation(IEnumerable<SmbResourceInfo> shares, Credentials credentials);
    bool TryDisconnectSharesFreeLetters(IEnumerable<SmbResourceInfo> shares);
    IEnumerable<SmbResourceInfo> GetConnectedShares();
    IEnumerable<SmbResourceInfo> GetServerConnectedShares(string server);
    IEnumerable<SmbResourceInfo> GetServerShares(string host, Credentials credentials);
}