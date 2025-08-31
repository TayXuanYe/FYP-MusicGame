using Godot;
using System.Text.Json;
using System.Threading.Tasks;

public partial class ApiClient : Node
{
	private static ApiClient _instance;
	public static ApiClient Instance => _instance;

	private string _apiBaseUrl;

	public override void _Ready()
	{
		_instance = this;
		LoadApiConfig();
	}

	private void LoadApiConfig()
	{
		if (FileAccess.FileExists("res://config.json"))
		{
			using var file = FileAccess.Open("res://config.json", FileAccess.ModeFlags.Read);
			var content = file.GetAsText();
			var jsonObject = Json.ParseString(content).As<Godot.Collections.Dictionary>();
			string apiBaseUrl = (string)jsonObject["api_base_url"];
			_apiBaseUrl = apiBaseUrl;
		}
	}

	public string BuildUrl(string endpoint)
	{
		return _apiBaseUrl + endpoint;
	}
}
