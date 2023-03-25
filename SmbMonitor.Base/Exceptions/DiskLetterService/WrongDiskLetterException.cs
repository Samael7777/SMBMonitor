namespace SmbMonitor.Base.Exceptions.DiskLetterService;

public class WrongDiskLetterException : DiskLetterServiceException
{
    public WrongDiskLetterException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public WrongDiskLetterException()
    { }

    public WrongDiskLetterException(string? message) : base(message)
    { }
}