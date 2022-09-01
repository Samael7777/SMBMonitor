using SMBMonitor.Model;

namespace SMBMonitor;

public static class ModelsContainer
{
    public static MainModel MainModel { get; }
    public static SettingsModel SettingsModel { get; }

    static ModelsContainer()
    {
        SettingsModel = new SettingsModel();
        MainModel = new MainModel(SettingsModel);
    }
}
