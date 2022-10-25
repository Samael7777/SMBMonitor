namespace SmbMonitorLib.Exceptions;

public class ItemNotExistsException : ServiceException
{
    public ItemNotExistsException()
    {
    }

    public ItemNotExistsException(string message) : base(message)
    {
    }
}