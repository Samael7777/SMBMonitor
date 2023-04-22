using System.Collections.Concurrent;
using System.Collections.Immutable;
using SmbMonitor.Base.DTO;
using SmbMonitor.Base.Exceptions.DiskLetterService;
using SmbMonitor.Base.Interfaces;
using SmbMonitor.Base.Interfaces.Tools;

namespace SmbMonitor.Domain.Services;



/// <summary>
///     Менеджер букв логических дисков
/// </summary>
/// <remarks>Выделяет свободную букву диска для монтирования</remarks>
public class DiskLettersService : IDiskLettersService
{
    private static readonly object _syncRoot = new();
    private readonly IMountedDiskLettersResolver _diskLettersResolver;
    private readonly ConcurrentDictionary<char, LetterUsageInfo> _letters;
    
    public DiskLettersService(IMountedDiskLettersResolver diskLettersResolver)
    {
        _letters = new ConcurrentDictionary<char, LetterUsageInfo>();
        _diskLettersResolver = diskLettersResolver;

        for (var letter = 'A'; letter <= 'Z'; letter++)
            _letters.TryAdd(letter, new LetterUsageInfo());
    }

    // ReSharper disable once InconsistentlySynchronizedField
    public IReadOnlyDictionary<char, LetterUsageInfo> Letters
    {
        get
        {
            UpdateUsedLetters();
            return _letters.ToImmutableDictionary();
        }
    }

    public void AddLetterReservation(char letter)
    {
        letter = char.ToUpper(letter);
        CheckArgument(letter);
        lock (_syncRoot)
        {
            _letters[letter].IsReserved = true;
        }
    }

    public void DeleteLetterReservation(char letter)
    {
        letter = char.ToUpper(letter);
        CheckArgument(letter);
        lock (_syncRoot)
        {
            _letters[letter].IsReserved = false;
        }
    }

    public char GetNextFreeLetterWithReservation()
    {
        char letter;
        UpdateUsedLetters();
        lock (_syncRoot)
        {
            letter = _letters
                .OrderBy(dic=>dic.Key)
                .FirstOrDefault(dic 
                    => !dic.Value.IsUsing && !dic.Value.IsReserved).Key;

            if (letter == (char)0)
                throw new NoFreeDiskLettersException();
            
            _letters[letter].IsReserved = true;
        }
        return letter;
    }
    
    public void UpdateUsedLetters()
    {
        lock (_syncRoot)
        {
            var usingLetters = _diskLettersResolver.GetMountedDiskLetters().ToList();

            foreach (var letterData in _letters)
            {
                letterData.Value.IsUsing = usingLetters.Contains(letterData.Key);
            }
        }
    }

    public void ClearLetterReservation()
    {
        lock (_syncRoot)
        {
            foreach (var letter in _letters.Keys)
            {
                _letters[letter].IsReserved = false;
            }
        }
    }

    private static void CheckArgument(char letter)
    {
        if (letter is < 'A' or > 'Z')
            throw new WrongDiskLetterException();
    }
}