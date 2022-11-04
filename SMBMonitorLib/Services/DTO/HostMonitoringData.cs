using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Services.DTO;

internal record HostMonitoringData
{
    public LastScanStatus LastScanStatus { get; set; } = LastScanStatus.Unknown;
    public bool IsScanning { get; set; } = false;
}