namespace SmbMonitorLib.Exceptions;

public class ItemExistsException : ServiceException
{
    public ItemExistsException()
    {
    }

    public ItemExistsException(string message) : base(message)
    {
    }
}