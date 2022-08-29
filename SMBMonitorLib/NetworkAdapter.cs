using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SmbMonitorLib;

/// <summary>
///     Класс сетевого интерфейса
/// </summary>
public class NetworkAdapter
{
    /// <summary>
    ///     Конструктор
    /// </summary>
    /// <param name="id">GUID адаптера</param>
    public NetworkAdapter(Guid id)
    {
        Id = id;
        UpdateInfo();
    }

    /// <summary>
    ///     GUID сетевого интерфейса
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     IPv4 адрес интерфейса
    /// </summary>
    public IPAddress InterfaceIp { get; private set; } = IPAddress.None;

    /// <summary>
    ///     Маска подсети IPv4
    /// </summary>
    public IPAddress SubnetMask { get; private set; } = IPAddress.None;

    /// <summary>
    ///     IPv4 адрес шлюза
    /// </summary>
    public IPAddress GatewayIp { get; private set; } = IPAddress.None;

    /// <summary>
    ///     Описание интерфейса
    /// </summary>
    public string Description { get; private set; } = "";

    /// <summary>
    ///     Состояние подключения к сети
    /// </summary>
    public OperationalStatus Status { get; private set; } = OperationalStatus.NotPresent;

    /// <summary>
    ///     Тип адаптера
    /// </summary>
    public NetworkInterfaceType InterfaceType { get; private set; } = NetworkInterfaceType.Unknown;

    /// <summary>
    ///     Обновить текущую информацию об интерфейсе
    /// </summary>
    public void UpdateInfo()
    {
        var adapter = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(i => Guid.Parse(i.Id) == Id);

        GatewayIp = adapter?.GetIPProperties().GatewayAddresses
                        .FirstOrDefault(u =>
                            u.Address.AddressFamily == AddressFamily.InterNetwork)?.Address
                    ?? IPAddress.None;

        InterfaceIp = adapter?.GetIPProperties().UnicastAddresses
                          .FirstOrDefault(u =>
                              u.Address.AddressFamily == AddressFamily.InterNetwork)?.Address
                      ?? IPAddress.None;
        SubnetMask = adapter?.GetIPProperties().UnicastAddresses
                         .FirstOrDefault(u =>
                             u.IPv4Mask.AddressFamily == AddressFamily.InterNetwork)?.IPv4Mask
                     ?? IPAddress.None;

        Description = adapter?.Description ?? string.Empty;
        Status = adapter?.OperationalStatus ?? OperationalStatus.NotPresent;
        InterfaceType = adapter?.NetworkInterfaceType ?? NetworkInterfaceType.Unknown;
    }
}