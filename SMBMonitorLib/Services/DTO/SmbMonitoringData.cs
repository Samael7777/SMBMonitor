using Base;
using SMBMonitorLib.Services.Base;

namespace SmbMonitorLib.Services.DTO;

public class SmbMonitoringData : ModelItem
{
    private SmbStatus _status = SmbStatus.Unknown;
    private bool _isHostAvailable;
    
    public Credentials Credentials { get; init; } = new("", "");

    public SmbStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public bool IsHostAvailable
    {
        get => _isHostAvailable;
        set
        {
            _isHostAvailable = value;
            OnPropertyChanged(nameof(IsHostAvailable));
        }
    }

    public int ConnectedShares { get; set; } = 0;


}