using SmbMonitor.Base.Interfaces.Tools;
using System.Net;
using System.Net.Sockets;

namespace SmbMonitor.Domain.Tools;

public class PortScanner : IPortScanner, IPortScannerAsync
{
    public bool IsPortOpen(IPAddress address, int port, int timeoutMs)
    {
        CheckScanParams(address, port, timeoutMs);
        var timeoutSpan = TimeSpan.FromMilliseconds(timeoutMs);
        bool result;
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var asyncResult = socket.BeginConnect(address, port, null, null);
            result = asyncResult.AsyncWaitHandle.WaitOne(timeoutSpan, true);
            
            if (result) 
                socket.EndConnect(asyncResult);
            
            socket.Close();
        }
        catch (SocketException)
        {
            result = false;
        }

        return result;
    }

    public async Task<bool> IsPortOpenAsync(IPAddress address, int port, int timeoutMs)
    {
        CheckScanParams(address, port, timeoutMs);
        
        var result = false;
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var token = new CancellationTokenSource(timeoutMs).Token;
            await socket.ConnectAsync(address, port, token);

            if (socket.Connected)
            {
                result = true;
                token = new CancellationTokenSource(timeoutMs).Token;
                await socket.DisconnectAsync(false, token);
            }
            socket.Close();
        }
        catch (OperationCanceledException)
        { }
        catch (SocketException)
        { }

        return result;
    }

    private static void CheckScanParams(IPAddress address, int port, int timeout)
    {
        if (IsIpWrong(address))
            throw new ArgumentOutOfRangeException(nameof(address), address, null);

        if (port is <= 0 or >= 65535)
            throw new ArgumentOutOfRangeException(nameof(port), port, null);

        if (timeout < 0)
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, null);
    }

    private static bool IsIpWrong(IPAddress address)
    {
        var wrongIPs = new[] { IPAddress.Any, IPAddress.Broadcast, IPAddress.None };
        return wrongIPs.Contains(address);
    }
}