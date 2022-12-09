using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Interfaces;

public interface ISmbMonitoringService : IControlledService, IBaseService
{
    void UpdateNodeStatus(MonitorNode node);
}