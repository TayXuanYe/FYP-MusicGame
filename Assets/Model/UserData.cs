// UserData.cs

using System.Text.Json.Serialization;
public class UserData
{
	public UserData() { }

	[JsonPropertyName("id")]
	public int? Id { get; set; }

	[JsonPropertyName("username")]
	public string Username { get; set; }

	[JsonPropertyName("email")]
	public string Email { get; set; }

	[JsonPropertyName("isLogin")]
	public bool IsLogin { get; set; }

	[JsonPropertyName("authToken")]
	public string AuthToken { get; set; }

	[JsonPropertyName("suggestedDifficulty")]
	public string SuggestedDifficulty { get; set; }

	[JsonPropertyName("masterVolume")]
	public string MasterVolume { get; set; } = "1.0";

	[JsonPropertyName("effectVolume")]
	public string EffectVolume { get; set; } = "1.0";

	[JsonPropertyName("musicVolume")]
	public string MusicVolume { get; set; } = "1.0";

	public UserData(int id, string username, string email, bool isLoggedIn, string authToken)
	{
		Id = id;
		Username = username;
		Email = email;
		IsLogin = isLoggedIn;
		AuthToken = authToken;
	}
}
