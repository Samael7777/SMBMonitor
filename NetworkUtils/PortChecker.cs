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
        _address = address;
        _port = port;
        _timeout = TimeSpan.FromMilliseconds(checkingTimeout);
    }

    public bool IsPortOpen()
    {
        if (_address.Equals(IPAddress.None)) return false;
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var asyncResult = socket.BeginConnect(_address, _port, null, null);
            var result = asyncResult.AsyncWaitHandle.WaitOne(_timeout, true);
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