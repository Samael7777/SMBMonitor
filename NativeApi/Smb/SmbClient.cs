using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NativeApi.Smb.Base;
using NativeApi.Smb.Win32;
using NativeApi.Smb.Win32.DTO;
using SmbAPI.Win32.DTO;
using static NativeApi.Smb.Win32.Mpr;

namespace NativeApi.Smb;

public static class SmbClient
{
    public static void ConnectNetworkDisk(string remotePath, char diskLetter,
        string user, string password)
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

        ConnectToRemoteServer(target, user, password, CONNECT_TEMPORARY);
    }

    public static void DisconnectNetworkDisk(string disk)
    {
        const bool forceDisconnectFlag = true;
        
        var regex = new Regex(@"^[A-z]:$");
        if (!regex.IsMatch(disk))
            throw new ArgumentException($"{disk} is not valid disk name.");

        CancelConnection(disk, 0, forceDisconnectFlag);
    }

    public static List<SmbResourceInfo> GetConnectedShares()
    {
        using var handle = OpenEnumConnectedShares();
        var resources = EnumResources(handle);
        var result = ConvertToNetworkDriveInfo(resources);
        return result;
    }

    public static List<SmbResourceInfo> GetRemoteShares(string server, string user, string password)
    {
        using var handle = OpenEnumRemoteShares(server, user, password);
        var resources = EnumResources(handle);
        var remoteShares = ConvertToNetworkDriveInfo(resources);
        return remoteShares.ToList();
    }

    public static LastErrorInfo GetLastErrorInfo()
    {
        return Mpr.GetLastErrorInfo();
    }

    private static SafeMprHandle OpenEnumRemoteShares(string smbServerName,
        string user, string password)
    {
        //const int CONNECT_CURRENT_MEDIA = 0x200;
        const int CONNECT_TEMPORARY = 0x04;

        var target = new NetResource
        {
            ResourceType = ResourceType.ResourcetypeDisk,
            ResourceScope = ResourceScope.ResourceGlobalnet,
            ResourceUsage = ResourceUsage.ResourceusageConnectable,
            RemoteName = smbServerName,
            LocalName = "",
            Comments = "",
            Provider = ""
        };
        ConnectToRemoteServer(target, user, password, CONNECT_TEMPORARY);

        var handle = OpenEnumByNetResource(target);

        return handle;
    }

    private static SafeMprHandle OpenEnumConnectedShares()
    {
        var handle = OpenLocalEnum
        (
            ResourceScope.ResourceConnected,
            ResourceType.ResourcetypeDisk,
            ResourceUsage.ResourceusageConnectable
        );

        return handle;
    }

    private static SafeMprHandle OpenEnumByNetResource(NetResource resource)
    {
        var handle = OpenRemoteEnum
        (
            resource.ResourceScope,
            resource.ResourceType,
            resource.ResourceUsage,
            resource
        );
        return handle;
    }

    private static List<NetResource> EnumResources(SafeMprHandle handle)
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

    private static List<NetResource> GetResourceList(SafeMprHandle handle)
    {
        const int requestAllData = -1;

        var bufferSize = 32768u;
        using var buffer = new MprSafeBuffer(bufferSize);

        var count = requestAllData;
        EnumResourcesToBuffer(handle, ref count, buffer, ref bufferSize);
        return CovertBufferToList(buffer, count);
    }

    private static List<NetResource> CovertBufferToList(MprSafeBuffer buffer, int elementsCount)
    {
        var result = new List<NetResource>();
        for (var i = 0; i < elementsCount; i++)
        {
            var item = BufferToItem(buffer, i);
            if (item != null) result.Add((NetResource)item);
        }
        return result;
    }

    private static NetResource? BufferToItem(MprSafeBuffer buffer, int index)
    {
        var itemSize = Marshal.SizeOf(typeof(NetResource));
        var mustCallRelease = false;
        NetResource? item;
        try
        {
            buffer.DangerousAddRef(ref mustCallRelease);
            var handle = buffer.DangerousGetHandle();
            var resourcePtr = IntPtr.Add(handle, index * itemSize);
            item = Marshal.PtrToStructure<NetResource>(resourcePtr);
        }
        finally
        {
            if(mustCallRelease) buffer.DangerousRelease();
        }
        return item;
    }

    private static void ConnectToRemoteServer(NetResource resource, string user, string password,
        int connectionFlag)
    {
        const int ERROR_SESSION_CREDENTIAL_CONFLICT = 0x4c3;
        
        try
        {
            AddConnection(resource, password, user, connectionFlag);
        }
        catch (Win32Exception e)
        {
            if (e.NativeErrorCode != ERROR_SESSION_CREDENTIAL_CONFLICT)
                throw;

            AddConnection(resource, null, null, connectionFlag);
        }
    }

    private static List<SmbResourceInfo> ConvertToNetworkDriveInfo(List<NetResource> source)
    {
        return source.ConvertAll((nr)=>nr.ToSmbResourceInfo());
    }
}