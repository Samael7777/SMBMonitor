using System.Collections.Concurrent;
using System.Net;
using SmbMonitor.Base.DTO.Smb;

namespace SmbMonitor.Base.DTO.Nodes;

public class HostNode : BaseNotification
{
    private SmbStatus _smbStatus;
    private SmbServerStatus _status;
    private bool _isScanning;
    private int _scanTries;

    public HostNode(IPHostEntry host, Credentials credentials = default, bool isManaged = true)
    {
        Host = host;
        IsManaged = isManaged;
        Credentials = credentials;
        AvailableShares = new ConcurrentDictionary<string, SmbResourceInfo>();
        ConnectedShares = new ConcurrentDictionary<string, SmbResourceInfo>();
    }

    public ConcurrentDictionary<string, SmbResourceInfo> AvailableShares { get; }
    public ConcurrentDictionary<string, SmbResourceInfo> ConnectedShares { get; }

    public bool IsManaged { get; }
    public virtual string Description => Host.HostName;
    public Credentials Credentials { get; set; }
    public IPHostEntry Host { get; }

    public SmbServerStatus SmbServerStatus
    {
        get => _status;
        set => SetField(ref _status, value);

    }
    public SmbStatus SmbStatus
    {
        get => _smbStatus;
        set => SetField(ref _smbStatus, value);
    }
    public bool IsScanning
    {
        get => _isScanning;
        set => SetField(ref _isScanning, value);
    }
    public int SmbCheckTries
    {
        get => _scanTries;
        set => SetField(ref _scanTries, value);
    }
}