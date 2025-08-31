public class UserData
{
    public int? Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsLoggedIn { get; set; }
    public string AuthToken { get; set; }

    public UserData(int id, string username, string email, bool isLoggedIn, string authToken)
    {
        Id = id;
        Username = username;
        Email = email;
        IsLoggedIn = isLoggedIn;
        AuthToken = authToken;
    }
}