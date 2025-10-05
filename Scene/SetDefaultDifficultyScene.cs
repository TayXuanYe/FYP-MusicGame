using Godot;
using System;

public partial class SetDefaultDifficultyScene : Control
{
	[Export] private Button _basicButton;
	[Export] private Button _advancedButton;
	[Export] private Button _expertButton;
	[Export] private HttpRequest _httpRequest;
	[Export] private Control _loadingComponent;

	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		_basicButton.Pressed += OnBasicButtonPressed;
		_advancedButton.Pressed += OnAdvancedButtonPressed;
		_expertButton.Pressed += OnExpertButtonPressed;
		_loadingComponent.Visible = false;
	}

	private void OnBasicButtonPressed()
	{
		SubmitDefaultDifficultyToServer("Basic");
	}

	private void OnAdvancedButtonPressed()
	{
		SubmitDefaultDifficultyToServer("Advanced");
	}

	private void OnExpertButtonPressed()
	{
		SubmitDefaultDifficultyToServer("Expert");
	}

	private void SubmitDefaultDifficultyToServer(string difficulty)
	{
		_loadingComponent.Visible = true;
		string suggestDifficultyUrl = ApiClient.Instance.BuildUrl($"users/{UserDataManager.Instance.CurrentUser.Id}/suggested-difficulty");
		// prepare headers for the request
		var headers = new string[]
		{
			"Content-Type: application/json",
			$"Authorization: Bearer {UserDataManager.Instance.CurrentUser.AuthToken}"
		};

		// prepare body for the request
		var data = new Godot.Collections.Dictionary
		{
			{"suggestedDifficulty", difficulty}
		};
		string body = Json.Stringify(data);

		// submit request
		_httpRequest.Request(suggestDifficultyUrl, headers, HttpClient.Method.Put, body);

		_loadingComponent.Visible = true;
		UserDataManager.Instance.CurrentUser.SuggestedDifficulty = difficulty;
	}

	private void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonResponse = System.Text.Encoding.UTF8.GetString(body);
		GD.Print($"Success Response body: {jsonResponse}");
		GD.Print($"Response code {responseCode}");
		if (responseCode == 200)
		{
			SceneManager.Instance.ChangeToMainMenuScene();
		}
	
		_loadingComponent.Visible = false;
	}
}
