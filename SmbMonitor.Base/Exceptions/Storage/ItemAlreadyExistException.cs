using SmbMonitor.Base.DTO.Nodes;

namespace SmbMonitor.Base.Exceptions.Storage;

public class ItemAlreadyExistException : StorageException
{
    public string? ItemDescription { get; }

    public ItemAlreadyExistException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public ItemAlreadyExistException()
    { }

    public ItemAlreadyExistException(string? message) : base(message)
    { }

    public ItemAlreadyExistException(HostNode node)
    {
        ItemDescription = node.Description;
    }
}
