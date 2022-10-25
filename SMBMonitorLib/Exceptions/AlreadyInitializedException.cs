namespace SmbMonitorLib.Exceptions;

public class AlreadyInitializedException : ServiceException
{
    public AlreadyInitializedException() : base("Service already initialized.")
    {
    }
}
