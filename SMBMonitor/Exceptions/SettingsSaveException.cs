using System;

namespace SMBMonitor.Exceptions;

public class SettingsSaveException : Exception
{
    public SettingsSaveException(string message) : base(message)
    {
    }
}
