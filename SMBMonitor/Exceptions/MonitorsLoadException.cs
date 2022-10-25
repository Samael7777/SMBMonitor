using System;

namespace SMBMonitor.Exceptions;

public class MonitorsLoadException : Exception
{
    public MonitorsLoadException(string message) : base(message)
    {
    }
}
