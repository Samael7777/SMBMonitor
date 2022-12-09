using System.Net;
using System.Net.Sockets;

namespace NetworkUtils;

public class PortChecker
{
    private readonly IPAddress _address;
    private readonly int _port;
    private readonly TimeSpan _timeout;

    public PortChecker(IPAddress address, int port, int checkingTimeout)
    {
        if (address.Equals(IPAddress.None))
            throw new ArgumentException("IP address is not valid.");

        _address = address;
        _port = port;
        _timeout = TimeSpan.FromMilliseconds(checkingTimeout);
    }

    public bool IsPortOpen()
    {
        var result = false;
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var asyncResult = socket.BeginConnect(_address, _port, null, null);
            result = asyncResult.AsyncWaitHandle.WaitOne(_timeout, true);
            if (result) socket.EndConnect(asyncResult);
            socket.Close();
        }
        catch (SocketException)
        { }

        return result;
    }

    public async Task<bool> IsPortOpenAsync(CancellationToken cancelToken)
    {
        var result = false;
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var asyncResult = socket.BeginConnect(_address, _port, null, null);

            var scanTask = Task.Run(() => asyncResult.AsyncWaitHandle.WaitOne(_timeout, true), cancelToken);
            var cancelTask = Task.Run(() => cancelToken.WaitHandle.WaitOne(_timeout, true), cancelToken);
            var resultTask = await Task.WhenAny(scanTask, cancelTask);

            if (resultTask == scanTask)
            {
                socket.EndConnect(asyncResult);
                result = scanTask.Result;
            }

            socket.Close();
        }
        catch (SocketException)
        {
            result = false;
        }

        return result;
    }
}