namespace SmbMonitor.Base.Exceptions.Storage;

public class StorageException : ServiceException
{
    public StorageException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public StorageException()
    { }

    public StorageException(string? message) : base(message)
    { }
}