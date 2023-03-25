using System.Net;
using SmbMonitor.Base.DTO.Smb;
using SmbMonitor.Base.DTO.Wifi;

namespace SmbMonitor.Base.DTO.Nodes;

public class WifiNode : HostNode
{
    private bool _isApConnected;
    private Guid _adapterId;

    public WifiNode(WifiSSID ssid, Credentials credentials = default)
        : base(new IPHostEntry(), credentials)
    {
        WifiSSID = ssid;
    }

    public override string Description => WifiSSID.ToString();
    public WifiSSID WifiSSID { get; }

    public Guid AdapterId
    {
        get => _adapterId; 
        set => SetField(ref _adapterId, value);
    }
    public bool IsAPConnected
    {
        get => _isApConnected;
        set => SetField(ref _isApConnected, value);
    }
}