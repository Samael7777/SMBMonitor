using System.IO;
using System.Linq;
using NUnit.Framework;
using SmbMonitor.Domain.Tools;

namespace SMBMonitor.Domain.Tests.Tools;

[TestFixture]
public class MountedDiskLetterResolverTests
{
    [Test]
    public void ResolverTest()
    {
        var resolver = new MountedDiskLettersResolver();
        var testingData = resolver.GetMountedDiskLetters()
            .OrderBy(l=>l);
        var facts = DriveInfo.GetDrives()
            .Select(di => char.ToUpper(di.Name[0]))
        .OrderBy(l => l);

        Assert.AreEqual(true, testingData.SequenceEqual(facts));
    }
}
