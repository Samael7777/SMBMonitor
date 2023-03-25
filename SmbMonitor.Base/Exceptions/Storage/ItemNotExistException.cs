using SmbMonitor.Base.DTO.Nodes;

namespace SmbMonitor.Base.Exceptions.Storage;

public class ItemNotExistException : StorageException
{
    public string? ItemDescription { get; }

    public ItemNotExistException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public ItemNotExistException()
    { }

    public ItemNotExistException(string? message) : base(message)
    { }

    public ItemNotExistException(HostNode node)
    {
        ItemDescription = node.Description;
    }
}
