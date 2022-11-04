﻿using System.ComponentModel;
using Base;
using NetworkUtils;
using SmbAPI;
using SmbAPI.Base;
using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Services.DTO;

namespace SmbMonitorLib.Services.Internal;

public static class SmbManager
{
    public static ILogger? Logger { get; set; }

    public static void ConnectSharesWithLettersReservation(IEnumerable<SmbResourceInfo> shares, Credentials credentials)
    {
        var requestList = new List<ShareConnectRequest>();
        foreach (var share in shares)
        {
            var diskLetter = DiskLettersManager.GetNextFreeLetter();
            DiskLettersManager.AddLetterReservation(diskLetter);
            var request = new ShareConnectRequest(share.RemoteName, diskLetter, credentials);
            requestList.Add(request);
        }

        Parallel.ForEach(requestList, ConnectShare);
    }

    public static void DisconnectAllShares(Host server)
    {
        var shares = GetServerConnectedShares(server);
        Parallel.ForEach(shares, DisconnectShareAndFreeLetter);
    }

    private static void ConnectShare(ShareConnectRequest shareConnectRequest)
    {
        var share = shareConnectRequest.Share.FullPathWithIP;
        var diskLetter = shareConnectRequest.DiskLetter;

        var message = $"SMB-ресурс {share}";
        try
        {
            SmbClient.ConnectNetworkDisk(share, diskLetter, shareConnectRequest.Credentials);
            message += $" подключен как диск {diskLetter}:";
        }
        catch (NoFreeLetterException)
        {
            message += " : невозможно подключить - нет свободных дисков.";
        }
        catch (Win32Exception e)
        {
            DiskLettersManager.DeleteLetterReservation(diskLetter);
            message += $" : ошибка подключения - {e.Message}";
        }

        LogWriteLine(message);
    }

    public static void DisconnectShareAndFreeLetter(SmbResourceInfo share)
    {
        if (string.IsNullOrEmpty(share.LocalName)) return;
        var disk = share.LocalName;
        var letter = disk[0];
        var remoteName = share.RemoteName.FullPathWithIP;

        LogWriteLine($"Отключение SMB-ресурса {remoteName} ...");

        var message = $"SMB-ресурс {remoteName}";
        
        try
        {
            CheckLetter(letter);
            SmbClient.DisconnectNetworkDisk(disk);
            DiskLettersManager.DeleteLetterReservation(letter);
            message += $" отключен от диска {disk}";
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

    public static IEnumerable<Host> GetConnectedServers()
    {
        try
        {
            var connectedShares = SmbClient.GetConnectedShares();
            return connectedShares.Select(data => new Host(data.RemoteName.ServerAddress))
                .Distinct();
        }
        catch (Win32Exception e)
        {
            LogWriteLine($"{nameof(GetConnectedServers)} : Ошибка : {e.Message} ");
            return Enumerable.Empty<Host>();
        }
    }

    public static IEnumerable<SmbResourceInfo> GetServerConnectedShares(Host server)
    {
        try
        {
            var connectedShares = SmbClient.GetConnectedShares();
            return connectedShares.Where(i => i.RemoteName.ServerAddress.Equals(server.IPAddress));
        }
        catch (Win32Exception e)
        {
            LogWriteLine($"{nameof(GetConnectedServers)} : Ошибка : {e.Message} ");
            return Enumerable.Empty<SmbResourceInfo>();
        }
    }

    public static IEnumerable<SmbResourceInfo> GetServerDisconnectedShares(Host server, Credentials credentials)
    {
        var availableShares = GetServerShares(server, credentials);
        var connectedShares = GetServerConnectedShares(server);
        var disconnectedShares = availableShares
            .Where(availableShare=>!connectedShares.Contains(availableShare));

        return disconnectedShares;
    }

    public static IEnumerable<SmbResourceInfo> GetServerShares(Host server, Credentials credentials)
    {
        var serverPath = @$"\\{server.IPAddress}";
        var shares = new List<SmbResourceInfo>();
        try
        {
            shares = SmbClient.GetRemoteShares(serverPath, credentials);
        }
        catch (Win32Exception e)
        {
            LogWriteLine($"Ошибка получения списка ресурсов {serverPath} : {e.Message}");
        }

        return shares;
    }

    private static void CheckLetter(char letter)
    {
        if (char.ToLower(letter) is < 'a' or > 'z')
            throw new WrongLetterException();
    }

    private static void LogWriteLine(string message)
    {
        const string prefix = nameof(SmbManager);
        Logger?.WriteFormattedLine($"{prefix} : {message}");
    }
}