using SmbMonitor.Base.DTO;

namespace SmbMonitor.Base.Interfaces;

public interface IDiskLettersService
{
    IReadOnlyDictionary<char, UsageInfo> Letters { get; }
    void AddLetterReservation(char letter);
    void DeleteLetterReservation(char letter);
    char GetNextFreeLetterWithReservation();
    void UpdateUsedLetters();
    void ClearLetterReservation();
}