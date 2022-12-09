using System.Reflection;
using Newtonsoft.Json;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.JsonConverters;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib;

internal class SettingsStorage
{
    public ISettings? Settings { get; set; }
    public List<MonitorNode>? MonitorNodes { get; set; }
}

public static class SettingsSerializer
{
    private static readonly JsonSerializerSettings jsonSettings;
    private static Dictionary<string, dynamic>? container;

    static SettingsSerializer()
    {
        jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new IPAddressConverter());
        jsonSettings.TypeNameHandling = TypeNameHandling.Auto;
        jsonSettings.Formatting = Formatting.Indented;
    }

    public static string SaveSettingsToJson()
    {
        container = new Dictionary<string, dynamic>();

        var settingsService = ServiceManager.GetService<ISettings>();
        if (settingsService is null)
            throw new ServiceDependencyException("Can't get Settings service");
        
        var storageService = ServiceManager.GetService<IStorageService>();
        if (storageService is null)
            throw new ServiceDependencyException("Can't get Storage service");
       
        var monitorNodes = storageService.MonitorNodes.Keys
            .Where(n => n.Type != NodeType.ExternalHost).ToList();
        
        container.Add("Settings", settingsService);
        container.Add("Nodes", monitorNodes);
        
        return JsonConvert.SerializeObject(container, jsonSettings);
    }

    public static void RestoreSettingsFromJson(string json)
    {
        container = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json, jsonSettings);
        if (container is null)
            throw new JsonException("Can't parse settings file.");

        if (!container.TryGetValue("Settings", out var settingsService))
            throw new JsonException("Can't parse settings file.");

        if (!container.TryGetValue("Nodes", out var nodes))
            throw new JsonException("Can't parse settings file.");

        RestoreSettings((ISettings)settingsService);
        RestoreNodesList((List<MonitorNode>) nodes);
    }

    private static void RestoreSettings(ISettings newSettings)
    {
        var settings = ServiceManager.GetService<ISettings>();
        if (settings is null)
            throw new ServiceDependencyException("Can't get settings service.");

        var properties = typeof(ISettings).GetProperties(BindingFlags.Public);
        foreach (var property in properties)
        {
            var value = property.GetValue(newSettings);
            property.SetValue(settings, value);
        }
    }

    private static void RestoreNodesList(List<MonitorNode> newNodes)
    {
        var storageService = ServiceManager.GetService<IStorageService>();
        if (storageService is null)
            throw new ServiceDependencyException("Can't get storage service.");

        foreach (var node in newNodes)
        {
            storageService.AddItem(node);
        }
    }
}
