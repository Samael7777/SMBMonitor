using SmbMonitor.Base.DTO.Smb;
using SmbMonitor.NativeApi.Smb.Win32.DTO;

namespace SmbMonitor.NativeApi.Smb.Extensions
{
    internal static class NetResourceExtensions
    {
        public static SmbResourceInfo ToSmbResourceInfo(this NetResource resource)
        {
            if (resource.RemoteName.Length == 0)
                throw new ArgumentException("NetResource data error.");

            return new SmbResourceInfo
            {
                RemoteName = new Uri(resource.RemoteName),
                Comments = new string(resource.Comments),
                MappedDisk = new string(resource.LocalName).ToUpper(),
                Provider = new string(resource.Provider) 
            };
        }
    }
}
