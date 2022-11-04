using System.Net;
using Base;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Services.DTO;

public class WifiMonitoringData : ModelItem
{
    private bool _isConnected;

    public WifiMonitoringData(Credentials credentials)
    {
        Credentials = credentials;
    }

    public Credentials Credentials { get; init; }
    public IPAddress IPAddress { get; set; } = IPAddress.None;
    public Guid AdapterId { get; set; } = Guid.Empty;

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnPropertyChanged(nameof(IsConnected));
        }
    }
}