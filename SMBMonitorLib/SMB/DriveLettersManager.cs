namespace SmbMonitorLib.SMB;

/// <summary>
///     Менеджер букв логических дисков
/// </summary>
/// <remarks>Выделяет свободную букву диска для монтирования</remarks>
public static class DriveLettersManager
{
    private static readonly List<char> availableLetters;
    private static readonly List<char> reservedLetters;

    static DriveLettersManager()
    {
        availableLetters = new List<char>();
        reservedLetters = new List<char>();
    }
    
    public static char GetNextFreeDriveLetter()
    {
        UpdateLetters();
        var freeLetter = availableLetters.FirstOrDefault();

        if (freeLetter == (char)0)
            throw new IndexOutOfRangeException("No more free drive letters!");

        return freeLetter;
    }

    /// <summary>
    ///     Возвращает список всех доступных букв дисков
    /// </summary>
    public static List<char> FreeDriveLetters
    {
        get
        {
            UpdateLetters();
            return availableLetters;
        }
    }

    /// <summary>
    ///     Проверка на доступность буквы диска
    /// </summary>
    /// <param name="letter">Проверяемая буква диска</param>
    /// <returns>True, если буква доступна и не зарезервирована</returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool IsLetterFree(char letter)
    {
        letter = CheckAndUpperLetter(letter);
        
        UpdateLetters();
        
        return availableLetters.Contains(letter);
    }

    /// <summary>
    ///     Список исключенных букв дисков
    /// </summary>
    public static List<char> GetReservedLetters()
    {
        return reservedLetters;
    }

    /// <summary>
    ///     Исключить из использования буквы дисков
    /// </summary>
    /// <param name="letter"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void AddLetterReservation(char letter)
    {
        
        letter = CheckAndUpperLetter(letter);

        if (!reservedLetters.Contains(letter))
            reservedLetters.Add(letter);
    }

    /// <summary>
    ///     Отменить резервирование буквы диска
    /// </summary>
    /// <param name="letter"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void DeleteLetterReservation(char letter)
    {
        letter = CheckAndUpperLetter(letter);

        reservedLetters.Remove(letter);
    }

    /// <summary>
    ///     Обновление доступных букв дисков
    /// </summary>
    private static void UpdateLetters()
    {
        availableLetters.Clear();

        FillAvailableLetterList();
        ExcludeUsingDriveLetters();
        ExcludeReservedDriveLetters();
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
            throw new ArgumentException("Неверно задана буква диска.");
        
        return letter;
    }
}