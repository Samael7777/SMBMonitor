namespace SmbMonitor.NativeApi.Smb;

public record LastErrorInfo(int Error, string ErrorMessage, string ProviderName);