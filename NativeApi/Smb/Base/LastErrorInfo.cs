namespace NativeApi.Smb.Base;

public record LastErrorInfo(int Error, string ErrorMessage, string ProviderName);