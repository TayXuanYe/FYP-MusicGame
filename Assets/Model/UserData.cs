public class UserData
{
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsLoggedIn { get; set; }

    public UserData(string username, string email, bool isLoggedIn)
    {
        Username = username;
        Email = email;
        IsLoggedIn = isLoggedIn;
    }
}