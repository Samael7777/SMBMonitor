﻿namespace SmbMonitorLib;

public static class Defaults
{
    public static int SmbPort => 445;
    public static Timings Timings => new (){ Interval = 3000, Timeout = 1000 };
}
