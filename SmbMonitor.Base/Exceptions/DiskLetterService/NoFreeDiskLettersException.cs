namespace SmbMonitor.Base.Exceptions.DiskLetterService;

public class NoFreeDiskLettersException : DiskLetterServiceException
{
    public NoFreeDiskLettersException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public NoFreeDiskLettersException()
    { }

    public NoFreeDiskLettersException(string? message) : base(message)
    { }
}