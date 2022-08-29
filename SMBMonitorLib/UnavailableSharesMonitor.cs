using System.ComponentModel;
using SmbMonitorLib.PortChecking;
using SmbMonitorLib.SMB;

namespace SmbMonitorLib;

public static class UnavailableSharesMonitor
{
    private static ILogger? logger;
    // ReSharper disable once NotAccessedField.Local
    private static Timer? pollingTimer;

    static UnavailableSharesMonitor()
    {
        IsStarted = false;
        ScanIntervalMs = 5000;
        DefaultSmbPort = 445;
        DefaultCheckTimeoutMs = 1000;
    }

    #region Propetries

    public static bool IsStarted { get; private set; }

    /// <remarks>По умолчанию 5000 мс</remarks>
    public static int ScanIntervalMs { get; set; }

    /// <remarks>По умолчанию 445 (SMB2)</remarks>
    public static int DefaultSmbPort { get; set; }

    /// <remarks>По умолчанию 1000 мс</remarks>
    public static int DefaultCheckTimeoutMs { get; set; }

    #endregion

    #region Public methods

    public static void StartMonitoring()
    {
        if (IsStarted)
            return;

        IsStarted = true;

        pollingTimer = new Timer
        (
            PollingProc,
            null,
            0,
            ScanIntervalMs
        );

    }

    public static void StopMonitoring()
    {
        if (!IsStarted)
            return;

        IsStarted = false;
        pollingTimer = null;
    }

    public static void SetLogger(ILogger Logger)
    {
        logger = Logger;
        logger.SetPrefix("Монитор недоступных сетевых дисков");
    }

    #endregion

    private static void PollingProc(object? args)
    {
        var shares = SmbClient.GetConnectedShares();
        var serverList = GetUniqueServers(shares);
        Parallel.ForEach(serverList, CheckAndDisconnectUnavailableShare);
    }

    private static IEnumerable<string> GetUniqueServers(List<SharedDisk> shares)
    {
        var serverList = new List<string>();
        foreach (var share in shares)
        {
            var server = share.RootPath.ToLower();
            if (server.Equals(string.Empty))
                continue;

            if (!serverList.Contains(server))
                serverList.Add(server);
        }
        return serverList;
    }

    private static void CheckAndDisconnectUnavailableShare(string serverName)
    {
        if (IsServerAccessible(serverName))
            return;

        logger?.WriteFormattedLine($"Сервер {serverName} более не доступен.");

        var connectedShares = SmbClient.GetConnectedShares();
        var sharesToDisconnect = connectedShares
            .Where(s => s.RootPath.Equals(serverName, StringComparison.OrdinalIgnoreCase));

        Parallel.ForEach(sharesToDisconnect, DisconnectUnaccessibleShare);
    }

    private static void DisconnectUnaccessibleShare(SharedDisk share)
    {
        var message = "Неизвестная ошибка";
        var diskLetter = share.LocalName[0];
        try
        {
            SmbClient.DisconnectNetworkDisk(diskLetter);
            message = $"Недоступный сетевой ресурс {share.RemoteName} " +
                      $"отключен от диска {diskLetter}:";
        }
        catch (Win32Exception e)
        {
            message = $"Ошибка отключения сетевого ресурса {share.RemoteName}" +
                      $" : {e.Message}";
        }
        finally
        {
            logger?.WriteFormattedLine(message);
        }
    }
    private static bool IsServerAccessible(string serverName)
    {
        var hostName = serverName.Trim('\\');
        var host = new Host(hostName, DefaultSmbPort);
        var checker = new PortChecker(host, DefaultCheckTimeoutMs);

        return checker.IsPortOpen();
    }

}

/*

    /// <summary>
    ///     Отключить все недоступные сетевые диски
    /// </summary>
    public void DeleteInaccessibleShares()
    {
        StringBuilder message = new();

        var request = SmbDriveManager.GetConnectedShares();

        var serverList = new List<string>(); //Список проверяемых серверов
        foreach (var drive in request)
        {
            var server = SmbDriveManager.GetHostNameFromPath(drive.RemoteName);
            //Составляем список уникальных серверов
            if (!serverList.Any(s => s.Equals(server, StringComparison.OrdinalIgnoreCase)))
                serverList.Add(server);
        }

        var killTasks = new Task<List<string>>[serverList.Count];
        var i = 0;
        //Создаем таски для проверки доступности и отключении шар каждого сервера
        foreach (var s in serverList)
        {
            var killTask = new Task<List<string>>(() => DetachSharesTask(s));
            killTasks[i++] = killTask;
            killTask.Start();
        }

        // ReSharper disable once CoVariantArrayConversion
        Task.WaitAll(killTasks); //Ожидание завершения всех задач

        //Сбор результатов
        message.AppendJoin(Environment.NewLine,
            killTasks.Aggregate(new List<string>(), (c, n) =>
            {
                c.AddRange(n.Result);
                return c;
            }));
    }

    /// <summary>
    ///     Тело задачи отключения сетевого ресурса
    /// </summary>
    /// <param name="serverName"></param>
    /// <returns></returns>
    private List<string> DetachSharesTask(string serverName)
    {
        var result = new List<string>();
        if (CheckServerAccess(serverName, DefaultSmbPort, DefaultTimeout))
            return result;
        
        var connectedShares = SmbDriveManager.GetConnectedShares();
        
        var requiredShares = connectedShares.Where(d
                => SmbDriveManager.GetHostNameFromPath(d.RemoteName)
                    .Equals(serverName, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!requiredShares.Any())
        {
            //Нечего отключать
            result.Add("Недоступные ресурсы не обнаружены");
            return result;
        }

        //Отключение недоступных дисков
        foreach (var s in requiredShares)
        {
            SmbDriveManager.DetachNetworkDisk(s.LocalName[0], true);
        }

        return result;
    }*/