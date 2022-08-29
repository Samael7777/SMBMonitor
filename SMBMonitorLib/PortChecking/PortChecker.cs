using System.Net;
using System.Net.Sockets;

namespace SmbMonitorLib.PortChecking;

/// <summary>
///     Сканер портов
/// </summary>
public class PortChecker
{
    public PortChecker(Host host, int checkingTimeout)
    {
        Host = host;
        Timeout = TimeSpan.FromMilliseconds(checkingTimeout);
    }

    /// <summary>
    ///     Сканируемый хост
    /// </summary>
    public Host Host { get; }

    /// <summary>
    ///     Таймаут соединения
    /// </summary>
    public TimeSpan Timeout { get; set; }

    public bool IsPortOpen()
    {
        if (Host.IP.Equals(IPAddress.None)) return false;
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var asyncResult = socket.BeginConnect(Host.IP, Host.Port, null, null);
            var result = asyncResult.AsyncWaitHandle.WaitOne(Timeout, true);
            if (result) socket.EndConnect(asyncResult);
            socket.Close();
            return result;
        }
        catch (SocketException)
        {
            return false;
        }
    }
}