using System;
using System.Windows.Forms;

namespace SMBMonitor.Exceptions;

public class MonitorsSaveException : Exception
{
    public MonitorsSaveException(string message) : base(message)
    {
    }    
}
