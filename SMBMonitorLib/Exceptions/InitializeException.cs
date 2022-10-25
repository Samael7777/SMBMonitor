namespace SmbMonitorLib.Exceptions;

public class InitializeException : ServiceException
{
    public InitializeException(string initializationMethod)
        : base($"You must run {initializationMethod} method first.") { }
}
