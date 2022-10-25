using System.Collections.Concurrent;
using SmbMonitorLib.Exceptions;

namespace SmbMonitorLib;

/// <summary>
///     Менеджер букв логических дисков
/// </summary>
/// <remarks>Выделяет свободную букву диска для монтирования</remarks>
public static class DiskLettersManager
{
    private static readonly object syncRoot = new();

    private static readonly ConcurrentList<char> availableLetters;
    private static readonly ConcurrentList<char> reservedLetters;

    static DiskLettersManager()
    {
        availableLetters = new ConcurrentList<char>();
        reservedLetters = new ConcurrentList<char>();
        availableLetters.Clear();
        FillAvailableLetterList();
    }

    public static void DeleteLetterReservation(char letter)
    {
        letter = CheckAndUpperLetter(letter);
        lock (syncRoot)
        {
            if (reservedLetters.Contains(letter))
                reservedLetters.Remove(letter);
        }
    }

    public static void AddLetterReservation(char letter)
    {
        letter = CheckAndUpperLetter(letter);
        lock (syncRoot)
        {
            if (!reservedLetters.Contains(letter))
                reservedLetters.Add(letter);
        }
    }

    public static char GetNextFreeLetter()
    {
        UpdateLetters();
        var freeLetter = availableLetters.FirstOrDefault();
        if (freeLetter == (char)0)
            throw new NoFreeLetterException("No more free drive letters!");

        return freeLetter;
    }

    private static void UpdateLetters()
    {
        lock (syncRoot)
        {
            availableLetters.Clear();
            FillAvailableLetterList();
            ExcludeUsingDriveLetters();
            ExcludeReservedDriveLetters();
        }
    }

    private static void FillAvailableLetterList()
    {
        for (var c = 'D'; c <= 'Z'; c++) availableLetters.Add(c);
    }

    private static void ExcludeUsingDriveLetters()
    {
        var usedDrives = DriveInfo.GetDrives();
        foreach (var d in usedDrives)
            availableLetters.Remove(d.Name[0]);
    }

    private static void ExcludeReservedDriveLetters()
    {
        foreach (var r in reservedLetters)
            availableLetters.Remove(r);
    }

    private static char CheckAndUpperLetter(char letter)
    {
        letter = char.ToUpper(letter);

        if (letter is < 'A' or > 'Z')
            throw new NotLetterException($"{letter} is not valid drive letter.");

        return letter;
    }
}