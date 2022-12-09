using System.ComponentModel;
using System.Net;
using NativeApi.NetworkUtils;
using NativeApi.Smb;
using NativeApi.Smb.Base;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Interfaces;
using SmbMonitorLib.Services.Base;

namespace SmbMonitorLib.Services;

internal class SmbService : BaseService<SmbService>, ISmbService
{
    private readonly IDiskLettersService _lettersService;

    public SmbService(IDiskLettersService lettersService, ISettings settings)
    {
        _lettersService = lettersService;
        Logger = settings.Logger;
    }

    public void ConnectShareWithLettersReservation(SmbResourceInfo share, Credentials credentials)
    {
        var diskLetter = _lettersService.GetNextFreeLetterWithReservation();

        var remoteName = share.RemoteName.FullPathWithIP;
        var message = $"SMB-ресурс {remoteName}";
        try
        {
            SmbClient.ConnectNetworkDisk(remoteName, diskLetter, credentials.User, credentials.Password);
            message += $" подключен как диск {diskLetter}:";
        }
        catch (NoFreeLetterException)
        {
            message += " : невозможно подключить - нет свободных дисков.";
        }
        catch (Win32Exception e)
        {
            _lettersService.DeleteLetterReservation(diskLetter);
            message += $" : ошибка подключения - {e.Message}";
        }

        LogWriteLine(message);
    }

    public void DisconnectAllShares(Host server)
    {
        var shares = GetServerConnectedShares(server);
        Parallel.ForEach(shares, DisconnectShareAndFreeLetter);
    }

    public void DisconnectShareAndFreeLetter(SmbResourceInfo share)
    {
        var disk = share.LocalName;
        if (string.IsNullOrEmpty(disk)) return;

        var letter = disk[0];
        var remoteName = share.RemoteName.FullPathWithIP;

        LogWriteLine($"Отключение SMB-ресурса {remoteName} ...");

        var message = $"SMB-ресурс {remoteName}";

        try
        {
            SmbClient.DisconnectNetworkDisk(disk);
            _lettersService.DeleteLetterReservation(letter);
            message += $" отключен от диска {letter}";
        }
        catch (WrongLetterException)
        {
            message += " ошибка определения буквы диска.";
        }
        catch (Win32Exception e)
        {
            message += $" ошибка отключения : {e.Message}";
        }

        LogWriteLine(message);
    }

    public List<Host> GetConnectedServers()
    {
        List<Host> result;
        try
        {
            var connectedShares = SmbClient.GetConnectedShares();
            result = connectedShares.Distinct().ToList()
                .ConvertAll(i => new Host(i.RemoteName.ServerAddress));
        }
        catch (Win32Exception e)
        {
            LogWriteLine($"{nameof(GetConnectedServers)} : Ошибка : {e.Message} ");
            result = new List<Host>();
        }
        return result;
    }

    public List<SmbResourceInfo> GetConnectedShares()
    {
        return SmbClient.GetConnectedShares();
    }
    public List<SmbResourceInfo> GetServerConnectedShares(Host server)
    {
        List<SmbResourceInfo> result;
        try
        {
            var connectedShares = SmbClient.GetConnectedShares();
            result = connectedShares
                .Where(i => i.RemoteName.ServerAddress.Equals(server.IPAddress)).ToList();
        }
        catch (Win32Exception e)
        {
            LogWriteLine($"{nameof(GetConnectedServers)} : Ошибка : {e.Message} ");
            result = new List<SmbResourceInfo>();
        }
        return result;
    }

    public List<SmbResourceInfo> GetServerDisconnectedShares(Host server, Credentials credentials)
    {
        var shares = GetServerShares(server, credentials);
        var connectedShares = GetServerConnectedShares(server);
        var disconnectedShares = shares.Except(connectedShares).ToList();

        return disconnectedShares;
    }

    public List<SmbResourceInfo> GetServerShares(Host server, Credentials credentials)
    {
        List<SmbResourceInfo> result;
        if (server.IPAddress.Equals(IPAddress.None)) return new List<SmbResourceInfo>();

        var serverPath = @$"\\{server.IPAddress}";
        
        try
        {
            result = SmbClient.GetRemoteShares(serverPath, credentials.User, credentials.Password);
        }
        catch (Win32Exception e)
        {
            LogWriteLine($"Ошибка получения списка ресурсов {serverPath} : {e.Message}");
            result = new List<SmbResourceInfo>();
        }
        return result;
    }
}