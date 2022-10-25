using System;

namespace SMBMonitor.Exceptions;

public class SettingsLoadException : Exception
{
    public SettingsLoadException(string message) : base(message)
    {
    }
}