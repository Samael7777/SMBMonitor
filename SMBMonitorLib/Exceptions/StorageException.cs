namespace SmbMonitorLib.Exceptions;

public class StorageException : ServiceException
{
    public StorageException()
    {
    }

    public StorageException(string message) : base(message)
    {
    }
}