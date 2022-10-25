using System.Management;
using SmbMonitorLib.Services.Base;
using SmbMonitorLib.Services.Interfaces;

namespace SmbMonitorLib.Services.Internal;

internal class WindowsSharesMonitoringService : ControlledService<WindowsSharesMonitoringService>,
    IWindowsSharesMonitoringService
{
    private static WindowsSharesMonitoringService? instance;
    private const string Query = @"select * from __InstanceOperationEvent within 1 where TargetInstance "
                                 + "isa 'Win32_LogicalDisk' and TargetInstance.DriveType = 4";

    private readonly ManagementEventWatcher _watcher;

    private WindowsSharesMonitoringService()
    {
        _watcher = new ManagementEventWatcher(new WqlEventQuery(Query));
        _watcher.EventArrived += OnWMIEvent;
    }

    public static WindowsSharesMonitoringService Instance
    {
        get
        {
            instance ??= new WindowsSharesMonitoringService();
            return instance;
        }
    }

    public event Action? OnConnectedSharesListChanged;

    protected override void OnStart()
    {
        _watcher.Start();
    }

    protected override void OnStop()
    {
        _watcher.Stop();
    }

    private void OnWMIEvent(object sender, EventArrivedEventArgs e)
    {
        const string creationEvent = "__InstanceCreationEvent";
        const string deletionEvent = "__InstanceDeletionEvent";

        var eventClassPath = e.NewEvent.ClassPath;
        switch (eventClassPath.RelativePath)
        {
            case creationEvent:
            case deletionEvent:
                OnConnectedSharesListChanged?.Invoke();
                break;
            default: return;
        }
    }
}