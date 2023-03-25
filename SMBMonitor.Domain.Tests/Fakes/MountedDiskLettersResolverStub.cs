using System.Collections.Generic;
using SmbMonitor.Base.Interfaces.Tools;

namespace SMBMonitor.Domain.Tests.Fakes;

internal class MountedDiskLettersResolverStub : IMountedDiskLettersResolver
{
    private readonly char[] _letters = { 'C', 'D', 'E' };
    public IEnumerable<char> GetMountedDiskLetters() => _letters;
}
