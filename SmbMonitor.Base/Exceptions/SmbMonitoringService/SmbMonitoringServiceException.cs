namespace SmbMonitor.Base.Exceptions.SmbMonitoringService;

public class SmbMonitoringServiceException : ServiceException
{
    public SmbMonitoringServiceException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public SmbMonitoringServiceException()
    { }

    public SmbMonitoringServiceException(string? message) : base(message)
    { }
}