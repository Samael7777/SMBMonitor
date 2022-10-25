using System;
using SMBMonitor.Model;

namespace SMBMonitor;

public class ModelsContainer
{
    private MonitorsModel? _monitorsModel;
    private SettingsModel? _settingsModel;

    private ModelsContainer()
    {
    }

    static ModelsContainer()
    {
        Instance = new ModelsContainer();
    }

    public static ModelsContainer Instance { get; }

    public MonitorsModel MonitorsModel
    {
        get
        {
            if (_monitorsModel == null)
                throw new NullReferenceException(nameof(MonitorsModel));
            
            return _monitorsModel;
        }
    }

    public SettingsModel SettingsModel
    {
        get
        {
            if (_settingsModel == null)
                throw new NullReferenceException(nameof(SettingsModel));
            
            return _settingsModel;
        }
    }

    public void BuildDependencies()
    {
        _settingsModel = new SettingsModel();
        _monitorsModel = new MonitorsModel();
    }
}
