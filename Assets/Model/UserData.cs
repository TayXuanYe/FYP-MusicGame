public class UserData
{
    public UserData() { }
    public int? Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsLogin { get; set; }
    public string AuthToken { get; set; }

    public UserData(int id, string username, string email, bool isLoggedIn, string authToken)
    {
        Id = id;
        Username = username;
        Email = email;
        IsLogin = isLoggedIn;
        AuthToken = authToken;
    }
}