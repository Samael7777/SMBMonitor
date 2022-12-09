namespace SmbMonitorLib.Services.Base;

public enum SmbStatus
{
    Unknown,
    Disconnecting,
    Disconnected,
    Connected,
    Connecting,
    PartiallyConnected,
    NoSharesAvailable,
    Updating
}

public enum SmbServerStatus
{
    Unknown,
    Connected,
    Disconnected
}
public class MonitorNodeInfo : ModelCollectionItem
{
    private bool _isApConnected;
    private SmbServerStatus _smbServerStatus;
    private SmbStatus _smbStatus;

    public MonitorNodeInfo(MonitorNode node)
    {
        _smbStatus = SmbStatus.Unknown;
        _smbServerStatus = SmbServerStatus.Unknown;
        LinkedNode = node;
    }
    public MonitorNode LinkedNode { get; }
    public Guid AdapterId { get; set; } = Guid.Empty;
    
    public bool IsScanning { get; set; }
    public int SmbCheckTries { get; set; } = 0;
    public SmbStatus SmbStatus
    {
        get => _smbStatus;
        set
        {
            if (value == _smbStatus) return;
            _smbStatus = value;
            OnPropertyChanged();
        }
    }
    public bool IsApConnected
    {
        get => _isApConnected;
        set
        {
            if (value == _isApConnected) return;
            _isApConnected = value;
            OnPropertyChanged();
        }
    }
    public SmbServerStatus SmbServerStatus
    {
        get => _smbServerStatus;
        set
        {
            if (value == _smbServerStatus) return;
            _smbServerStatus = value;
            OnPropertyChanged();
        }
    }
}
