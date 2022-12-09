using Newtonsoft.Json;
using System.Net;

namespace SmbMonitorLib.JsonConverters;

internal class IPAddressConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IPAddress).IsAssignableFrom(objectType);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null) return;
        writer.WriteValue(value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.Value is null 
            ? IPAddress.None 
            : IPAddress.Parse((string)reader.Value);
    }
}
