using SMBMonitorLib.Services.Base;

namespace SmbMonitorLib.Services.DTO;

internal record HostMonitoringInfo
{
    public LastScanStatus LastScanStatus { get; set; } = LastScanStatus.Unknown;
    public bool IsScanning { get; set; } = false;
}