namespace SmbMonitor.Base.Exceptions.SmbMonitoringService;

public class UnknownMountedShareException : SmbMonitoringServiceException
{
    public UnknownMountedShareException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public UnknownMountedShareException()
    { }

    public UnknownMountedShareException(string? message) : base(message)
    { }
}