using System.ComponentModel;
using SmbMonitor.Base.DTO.Smb;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Tools;

public class SmbManager : ISmbManager
{
    private readonly ISmbClient _smbClient;
    private readonly IDiskLettersService _diskLettersService;

    public SmbManager(ISmbClient smbClient, IDiskLettersService diskLettersService)
    {
        _smbClient = smbClient;
        _diskLettersService = diskLettersService;
    }

    public bool TryConnectSharesWithAutoLetterReservation(IEnumerable<SmbResourceInfo> shares, Credentials credentials)
    {
        return TryExecuteSmbCommandParallel(shares, s =>
            TryConnectShareAutoReserveLetter(s, credentials));
    }

    public bool TryDisconnectSharesFreeLetters(IEnumerable<SmbResourceInfo> shares)
    {
        return TryExecuteSmbCommandParallel(shares, TryDisconnectShareReleaseLetter);
    }

    public IEnumerable<SmbResourceInfo> GetConnectedShares()
    {
        List<SmbResourceInfo> result;
        try
        {
            result = _smbClient.GetConnectedShares();
        }
        catch (Win32Exception)
        {
            result = new List<SmbResourceInfo>();
        }

        return result;
    }

    public IEnumerable<SmbResourceInfo> GetServerConnectedShares(string server)
    {
        CheckAndFormatHostName(ref server);

        var connectedShares = GetConnectedShares();
        var result = connectedShares
            .Where(i =>
                i.RemoteName.DnsSafeHost.Equals(server, StringComparison.OrdinalIgnoreCase));
        return result;
    }

    public IEnumerable<SmbResourceInfo> GetServerShares(string host, Credentials credentials)
    {
        CheckAndFormatHostName(ref host);

        List<SmbResourceInfo> result;

        var serverPath = $"\\\\{host}";
        try
        {
            result = _smbClient.GetRemoteShares(serverPath, credentials.User, credentials.Password);
        }
        catch (Win32Exception)
        {
            result = new List<SmbResourceInfo>();
        }

        return result;
    }

    private bool TryConnectShareAutoReserveLetter(SmbResourceInfo share, Credentials credentials)
    {
        const char noLetter = (char)0;

        var letter = noLetter;
        var result = true;
        try
        {
            letter = _diskLettersService.GetNextFreeLetterWithReservation();
            _smbClient.ConnectNetworkDisk(share.RemoteName, letter, credentials.User, credentials.Password);
        }
        catch (Exception)
        {
            result = false;
            if (letter != noLetter)
                _diskLettersService.DeleteLetterReservation(letter);
        }

        return result;
    }

    private bool TryDisconnectShareReleaseLetter(SmbResourceInfo share)
    {
        Console.WriteLine($"Disconnecting share {share.RemoteName.AbsolutePath}");
        
        if (string.IsNullOrWhiteSpace(share.MappedDisk))
            return true;

        var result = true;
        try
        {
            _smbClient.DisconnectNetworkDisk(share.MappedDisk);
            _diskLettersService.DeleteLetterReservation(share.MappedDisk[0]);
        }
        catch
        {
            result = false;
        }
        return result;
    }

    private static bool TryExecuteSmbCommandParallel(IEnumerable<SmbResourceInfo> shares, Func<SmbResourceInfo, bool> command)
    {
        var taskList = shares.Select(share =>
            {
                var task = new Task<bool>(() => command(share));
                task.Start();
                return task;
            });

        var taskResults = Task.WhenAll(taskList).Result;

        return taskResults.Aggregate(false, (s, a) => a || s);
    }

    private static void CheckAndFormatHostName(ref string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException(null, nameof(host));
        }

        host = host.Trim('\\');
    }
}