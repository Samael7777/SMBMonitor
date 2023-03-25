using System.Linq;
using NUnit.Framework;
using SmbMonitor.Base.Exceptions.DiskLetterService;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.Interfaces.Tools;
using SmbMonitor.Domain.Services;
using SMBMonitor.Domain.Tests.Fakes;

namespace SMBMonitor.Domain.Tests.Services.DiskLettersServiceTests;

[TestFixture]
public class DiskLettersServiceTests
{
    private readonly IDiskLettersService _diskLettersService;
    private readonly IMountedDiskLettersResolver _diskLettersResolver;
    public DiskLettersServiceTests()
    {
        _diskLettersResolver = new MountedDiskLettersResolverStub();
        _diskLettersService = new DiskLettersService(_diskLettersResolver);
    }

    [SetUp]
    public void SetUp()
    {
        _diskLettersService.ClearLetterReservation();
    }

    [Test]
    public void UpdateFreeLettersTest()
    {
        var usedLetters = _diskLettersResolver.GetMountedDiskLetters();

        _diskLettersService.UpdateUsedLetters();

        var testData = _diskLettersService.Letters
            .Where(info => info.Value.IsUsing)
            .Select(pair => pair.Key)
            .OrderBy(c => c);

        Assert.AreEqual(true, usedLetters.SequenceEqual(testData));
    }

    [Test]
    public void CorrectLetterReservationTest([ValueSource(typeof(DriveLettersSource), nameof(DriveLettersSource.CorrectLetters))] int letterInt)
    {
        var letter = char.ToUpper((char)letterInt);
        _diskLettersService.AddLetterReservation(letter);
        Assert.AreEqual(true, _diskLettersService.Letters[letter].IsReserved);
    }

    [Test]
    public void IncorrectLetterReservationTest([ValueSource(typeof(DriveLettersSource), nameof(DriveLettersSource.IncorrectLetters))] int letter)
    {
        Assert.Throws<WrongDiskLetterException>(() => 
            _diskLettersService.AddLetterReservation((char)letter));
    }

    [Test]
    public void IncorrectLetterDeleteReservationTest([ValueSource(typeof(DriveLettersSource), 
        nameof(DriveLettersSource.IncorrectLetters))] int letter)
    {
        Assert.Throws<WrongDiskLetterException>(() => 
            _diskLettersService.DeleteLetterReservation((char)letter));
    }

    [Test]
    public void ClearReservationTest()
    {
        foreach (var letter in DriveLettersSource.CorrectLetters)
        {
            _diskLettersService.AddLetterReservation((char)letter);
        }

        _diskLettersService.ClearLetterReservation();

        foreach (var letterData in _diskLettersService.Letters)
        {
            Assert.AreEqual(false, letterData.Value.IsReserved);
        }
    }

    [Test]
    public void DeleteLetterReservationTest([ValueSource(typeof(DriveLettersSource), 
            nameof(DriveLettersSource.CorrectLetters))] int letterInt)
    {
        var letter = (char)letterInt;
        _diskLettersService.AddLetterReservation(letter);

        _diskLettersService.DeleteLetterReservation(letter);

        Assert.AreEqual(false, 
            _diskLettersService.Letters[char.ToUpper(letter)].IsReserved);
    }

    [Test]
    public void GetNextFreeLetterWithReservationTest()
    {
        var letters = Enumerable.Range('A', 'Z' - 'A' + 1)
            .Select(i => (char)i)
            .OrderBy(l => l)
            .ToList();

        var usingLetters = _diskLettersService.Letters
            .Where(li => li.Value.IsUsing)
            .Select(li => li.Key);

        foreach (var letter in usingLetters) 
            letters.Remove(letter);

        foreach (var testLetter in letters)
        {
            var nextLetter = _diskLettersService.GetNextFreeLetterWithReservation();
            Assert.AreEqual(testLetter, nextLetter);
        }
    }

    [Test]
    public void NoFreeLetterTest()
    {
        var nextLetter = 'A';
        while (nextLetter < 'Z')
        {
            nextLetter = _diskLettersService.GetNextFreeLetterWithReservation();
        }

        Assert.Throws<NoFreeDiskLettersException>(() => 
            _diskLettersService.GetNextFreeLetterWithReservation());
    }
}