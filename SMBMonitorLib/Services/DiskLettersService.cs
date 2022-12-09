using SmbMonitorLib.Exceptions;
using SmbMonitorLib.Interfaces;

namespace SmbMonitorLib.Services;

internal record LetterInfo
{
    public bool IsReserved { get; set; }
    public bool IsUsed { get; set; }
}

/// <summary>
///     Менеджер букв логических дисков
/// </summary>
/// <remarks>Выделяет свободную букву диска для монтирования</remarks>
internal class DiskLettersService : IDiskLettersService
{
    private readonly SortedDictionary<char, LetterInfo> _letters;

    public DiskLettersService()
    {
        _letters = new SortedDictionary<char, LetterInfo>();
        FillLetters();
    }

    public void AddLetterReservation(char letter)
    {
        letter = char.ToUpper(letter);
        CheckArgument(letter);

        lock (_letters)
        {
            _letters[letter].IsReserved = true;
        }
    }

    public void AddLetterReservation(IEnumerable<char> letters)
    {
        foreach (var letter in letters) AddLetterReservation(letter);
    }

    public void DeleteLetterReservation(char letter)
    {
        letter = char.ToUpper(letter);
        CheckArgument(letter);

        lock (_letters)
        {
            _letters[letter].IsReserved = false;
        }
    }

    public char GetNextFreeLetter()
    {
        char letter;
        lock (_letters)
        {
            letter = GetNextFreeLetterThreadUnsafe();
        }

        if (letter == (char)0)
            throw new InvalidOperationException("No free letters");
        return letter;
    }

    public char GetNextFreeLetterWithReservation()
    {
        char letter;
        lock (_letters)
        {
            letter = GetNextFreeLetterThreadUnsafe();
            if (letter != (char)0)
                _letters[letter].IsReserved = true;
        }

        if (letter == (char)0)
            throw new NoFreeLetterException("No free drive letters left.");

        return letter;
    }

    private char GetNextFreeLetterThreadUnsafe()
    {
        UpdateUsedLetters();
        return _letters
            .FirstOrDefault(l => !l.Value.IsUsed && !l.Value.IsReserved).Key;
    }

    private void FillLetters()
    {
        for (var letter = 'A'; letter <= 'Z'; letter++)
            _letters.TryAdd(letter, new LetterInfo());
    }

    private void UpdateUsedLetters()
    {
        var drives = DriveInfo.GetDrives();

        foreach (var letter in _letters)
            letter.Value.IsUsed = false;

        foreach (var info in drives)
        {
            var usedLetter = char.ToUpper(info.Name[0]);
            _letters[usedLetter].IsUsed = true;
        }
    }

    private void CheckArgument(char letter)
    {
        if (letter is < 'A' or > 'Z')
            throw new WrongLetterException("Wrong letter.");
    }
}