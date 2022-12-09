namespace SmbMonitorLib.Exceptions;

public class ServiceDependencyException : ServiceException
{
    public ServiceDependencyException()
    { }

    public ServiceDependencyException(string message) : base(message)
    { }
}