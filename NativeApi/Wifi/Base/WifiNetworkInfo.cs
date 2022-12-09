namespace NativeApi.Wifi.Base;

public readonly record struct WifiNetworkInfo
{
    public Guid AssociatedAdapterId { get; init; }

    public WifiSSID Id { get; init; }

    public BssType BssType { get; init; }

    public int SignalQuality { get; init; }

    public bool IsSecurityEnabled { get; init; }

    public string ProfileName { get; init; }

    public AuthenticationAlgorithm AuthenticationAlgorithm { get; init; }

    public CipherAlgorithm CipherAlgorithm { get; init; }

    public ConnectionFlags Flags { get; init; }
}