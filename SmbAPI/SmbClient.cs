using System.ComponentModel;
using System.Runtime.InteropServices;

using System.Text.RegularExpressions;
using Base;
using SmbAPI.Base;
using SmbAPI.Win32;
using SmbAPI.Win32.DTO;
using static SmbAPI.Win32.Mpr;

namespace SmbAPI;

public static class SmbClient
{
    public static void ConnectNetworkDisk(string remotePath, char diskLetter,
        Credentials credentials)
    {
        const int CONNECT_TEMPORARY = 0x04;

        if (diskLetter is < 'A' or > 'z')
            throw new ArgumentException($"{diskLetter} must be valid disk letter.");

        NetResource target = new()
        {
            ResourceType = ResourceType.ResourcetypeDisk,
            LocalName = diskLetter + ":",
            RemoteName = remotePath.TrimEnd('\\')
        };

        ConnectToRemoteServer(target, credentials, CONNECT_TEMPORARY);
    }

    public static void DisconnectNetworkDisk(string disk)
    {
        var regex = new Regex(@"^[A-z]:$");
        if (!regex.IsMatch(disk))
            throw new ArgumentException($"{disk} is not valid disk name.");

        const bool forceDisconnectFlag = true;
        CancelConnection(disk, 0, forceDisconnectFlag);
    }

    public static IEnumerable<SmbResourceInfo> GetConnectedShares()
    {
        using var handle = OpenEnumConnectedShares();
        var resources = EnumResources(handle);
        var result = ConvertToNetworkDriveInfo(resources);
        return result;
    }

    public static List<SmbResourceInfo> GetRemoteShares(string server, Credentials credentials)
    {
        using var handle = OpenEnumRemoteShares(server, credentials);
        var resources = EnumResources(handle);
        var remoteShares = ConvertToNetworkDriveInfo(resources);
        return remoteShares.ToList();
    }

    public static LastErrorInfo GetLastErrorInfo()
    {
        return Mpr.GetLastErrorInfo();
    }

    #region Private

    private static SafeMprHandle OpenEnumRemoteShares(string smbServerName,
        Credentials credentials)
    {
        //const int CONNECT_CURRENT_MEDIA = 0x200;
        const int CONNECT_TEMPORARY = 0x04;

        var target = new NetResource
        {
            ResourceType = ResourceType.ResourcetypeDisk,
            ResourceScope = ResourceScope.ResourceGlobalnet,
            ResourceUsage = ResourceUsage.ResourceusageConnectable,
            RemoteName = smbServerName
        };
        ConnectToRemoteServer(target, credentials, CONNECT_TEMPORARY);

        var handle = OpenEnumByNetResource(target);

        return handle;
    }

    private static SafeMprHandle OpenEnumConnectedShares()
    {
        var source = new NetResource
        {
            ResourceType = ResourceType.ResourcetypeDisk,
            ResourceScope = ResourceScope.ResourceConnected,
        };

        var handle = OpenEnum
        (
            source.ResourceScope,
            source.ResourceType,
            source.ResourceUsage,
            null
        );

        return handle;
    }

    private static SafeMprHandle OpenEnumByNetResource(NetResource resource)
    {
        var handle = OpenEnum
        (
            resource.ResourceScope,
            resource.ResourceType,
            resource.ResourceUsage,
            resource
        );
        return handle;
    }

    private static IEnumerable<NetResource> EnumResources(SafeMprHandle handle)
    {
        var resourcesList = new List<NetResource>();

        var resources = GetResourceList(handle);

        foreach (var resource in resources)
            if (resource.ResourceUsage == ResourceUsage.ResourceusageContainer)
            {
                using var subHandle = OpenEnumByNetResource(resource);
                var subItems = EnumResources(subHandle);
                resourcesList.AddRange(subItems);
            }
            else if (resource.ResourceType == ResourceType.ResourcetypeDisk)
            {
                resourcesList.Add(resource);
            }

        return resourcesList;
    }

    private static IEnumerable<NetResource> GetResourceList(SafeMprHandle handle)
    {
        const int requestAllData = -1;
        var bufferSize = 1048576; //Размер буфера результатов

        using var buffer = new HeapBuffer(bufferSize);

        var count = requestAllData;

        EnumResourcesToBuffer(handle, ref count, buffer.Buffer, ref bufferSize);

        var resources = CovertBufferToEnumerable(buffer.Buffer, count);
        return resources;
    }

    private static IEnumerable<NetResource> CovertBufferToEnumerable(IntPtr buffer, int elementsCount)
    {
        var result = new List<NetResource>();

        for (var i = 0; i < elementsCount; i++)
        {
            var resourcePtr = IntPtr.Add(buffer, i * Marshal.SizeOf(typeof(NetResource)));

            var resource = Marshal.PtrToStructure<NetResource>(resourcePtr);
            if (resource != null)
                result.Add(resource);
        }

        return result;
    }

    private static IEnumerable<SmbResourceInfo> ConvertToNetworkDriveInfo
        (IEnumerable<NetResource> source)
    {
        return source.Select(nr => new SmbResourceInfo(nr));
    }

    private static void ConnectToRemoteServer(NetResource resource, Credentials credentials,
        int connectionFlag)
    {
        const int ERROR_SESSION_CREDENTIAL_CONFLICT = 0x4c3;

        try
        {
            AddConnection(resource, credentials.Password, credentials.User, connectionFlag);
        }
        catch (Win32Exception e)
        {
            if (e.NativeErrorCode != ERROR_SESSION_CREDENTIAL_CONFLICT)
                throw;

            AddConnection(resource, null, null, connectionFlag);
        }
    }

    #endregion
}