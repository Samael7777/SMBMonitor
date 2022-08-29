using System.ComponentModel;
using System.Runtime.InteropServices;
using SmbMonitorLib.SMB.Win32;

using static SmbMonitorLib.SMB.Win32.Mpr;

namespace SmbMonitorLib.SMB;

public static class SmbClient
{

    #region Public

    public static void ConnectNetworkDisk(string remotePath, char driveLetter,
        Credentials credentials)
    {
        const int CONNECT_TEMPORARY = 0x04;

        NetResource target = new()
        {
            ResourceType = ResourceType.ResourcetypeDisk,
            LocalName = driveLetter + ":",
            RemoteName = remotePath.TrimEnd('\\')
        };

        LoginToRemoteServer(target, credentials, CONNECT_TEMPORARY);
    }

    public static void DisconnectNetworkDisk(char driveLetter)
    {
        const bool forceDisconnectFlag = true;
        var drive = $"{driveLetter}:";

        CancelConnection(drive, 0, forceDisconnectFlag);
    }

    public static List<SharedDisk> GetConnectedShares()
    {
        using var handle = OpenEnumConnectedShares();
        var resources = EnumResources(handle);
        var result = ConvertToNetworkDriveInfo(resources);

        return result.ToList();
    }

    public static List<SharedDisk> GetRemoteShares(string smbServerName,
            Credentials credentials)
    {
        using var handle = OpenEnumRemoteShares(smbServerName, credentials);
        var resources = EnumResources(handle);
        var result = ConvertToNetworkDriveInfo(resources);

        return result.ToList();
    }

    #endregion

    #region Private

    private static SafeMprHandle OpenEnumRemoteShares(string smbServerName,
       Credentials credentials)
    {
        //const int CONNECT_CURRENT_MEDIA = 0x200;
        const int CONNECT_TEMPORARY = 0x04;

        var target = new NetResource()
        {
            ResourceType = ResourceType.ResourcetypeDisk,
            ResourceScope = ResourceScope.ResourceGlobalnet,
            ResourceUsage = ResourceUsage.ResourceusageConnectable,
            RemoteName = smbServerName.TrimEnd('\\')
        };

        LoginToRemoteServer(target, credentials, CONNECT_TEMPORARY);

        var handle = OpenEnumByNetResource(target);

        return handle;
    }
    private static SafeMprHandle OpenEnumConnectedShares()
    {
        var source = new NetResource()
        {
            ResourceType = ResourceType.ResourcetypeDisk,
            ResourceScope = ResourceScope.ResourceConnected,
            ResourceUsage = ResourceUsage.ResourceusageAttached
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
        {
            if (resource.ResourceUsage == ResourceUsage.ResourceusageContainer)
            {
                using var subHandle = OpenEnumByNetResource(resource);
                var subItems = EnumResources(subHandle);
                resourcesList.AddRange(subItems);

            }
            else if (resource.ResourceType == ResourceType.ResourcetypeDisk)
                resourcesList.Add(resource);
        }

        return resourcesList;
    }

    private static IEnumerable<NetResource> GetResourceList(SafeMprHandle handle)
    {
        const int requestAllData = -1;
        var bufferSize = 1048576; //Размер буфера результатов (1 Мб)

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

    private static IEnumerable<SharedDisk> ConvertToNetworkDriveInfo
        (IEnumerable<NetResource> source)
    {
        return source.Select(nr => new SharedDisk(nr));
    }
    private static void LoginToRemoteServer(NetResource resource, Credentials credentials,
            int connectionFlag)
    {
        const int ERROR_SESSION_CREDENTIAL_CONFLICT = 0x4c3;

        try
        {
            AddConnection
            (
                resource,
                credentials.Password,
                credentials.User,
                connectionFlag
            );
        }
        catch (Win32Exception e)
        {
            if (e.NativeErrorCode != ERROR_SESSION_CREDENTIAL_CONFLICT)
                throw;
            AddConnection
            (
                resource,
                null,
                null,
                connectionFlag
            );
        }
    } 

    #endregion
}
