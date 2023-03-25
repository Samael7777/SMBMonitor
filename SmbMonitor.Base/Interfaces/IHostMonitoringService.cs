using SmbMonitor.Base.DTO.Nodes;

namespace SmbMonitor.Base.Interfaces;

//TODO Пересмотреть интерфейс

public interface IHostMonitoringService
{
    void DoPortScan(IEnumerable<HostNode> nodes);
}