namespace SmbMonitorLib;

public class Credentials
{
    public string User { get; set; }
    public string Password { get; set; }

    public Credentials()
    {
        User = string.Empty;
        Password = string.Empty;
    }

    public Credentials(string user, string password)
    {
        User = user;
        Password = password;
    }
}
