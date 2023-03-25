using System.Collections.Generic;

namespace SMBMonitor.Domain.Tests.Services.DiskLettersServiceTests;

internal static class DriveLettersSource
{
    public static IEnumerable<int> IncorrectLetters => new[] { 0, 'A' - 1, 'z' + 1, char.MaxValue - 1, ' ' };
    public static IEnumerable<int> CorrectLetters => new[] { (int)'A', 'z', 'f', 'K', 'c', 'e' };
}
