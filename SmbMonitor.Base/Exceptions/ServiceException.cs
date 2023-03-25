namespace SmbMonitor.Base.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public ServiceException()
    { }

    public ServiceException(string? message) : base(message)
    { }
}