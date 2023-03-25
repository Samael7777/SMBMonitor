namespace SmbMonitor.Base.Exceptions.DiskLetterService;

public class DiskLetterServiceException : ServiceException
{
    public DiskLetterServiceException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public DiskLetterServiceException()
    { }

    public DiskLetterServiceException(string? message) : base(message)
    { }
}