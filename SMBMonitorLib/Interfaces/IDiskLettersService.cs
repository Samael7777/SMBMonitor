namespace SmbMonitorLib.Interfaces;

public interface IDiskLettersService
{
    void AddLetterReservation(char letter);
    void AddLetterReservation(IEnumerable<char> letters);
    void DeleteLetterReservation(char letter);
    char GetNextFreeLetter();
    char GetNextFreeLetterWithReservation();
}