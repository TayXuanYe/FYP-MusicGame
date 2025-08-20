public class UserData
{
    public string Username { get; set; }
    public string Email { get; set; }

    public UserData(string username, string email)
    {
        Username = username;
        Email = email;
    }
}