using Godot;
using System;

public partial class SettingScene : Control
{
	[Export] private Button _applyChangesButton;
	[Export] private Button _backButton;
	[Export] private HSlider _masterVolumeSlider;
	[Export] private HSlider _effectVolumeSlider;
	[Export] private HSlider _musicVolumeSlider;
	[Export] private Label _masterVolumeLabel;
	[Export] private Label _effectVolumeLabel;
	[Export] private Label _musicVolumeLabel;
	[Export] private Control _loadingComponent;
	[Export] private HttpRequest _httpRequest;
	[Export] private Control _messageComponentInstance;
	private MessageComponent _messageComponentScript;

	public override void _Ready()
	{
		_applyChangesButton.Pressed += OnApplyChangesButtonPressed;
		_backButton.Pressed += OnBackButtonPressed;
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;

		// Initialize sliders with current volume settings
		_masterVolumeSlider.Value = UserDataManager.Instance.CurrentUser.MasterVolume * 100;
		_effectVolumeSlider.Value = UserDataManager.Instance.CurrentUser.EffectVolume * 100;
		_musicVolumeSlider.Value = UserDataManager.Instance.CurrentUser.MusicVolume * 100;

		_loadingComponent.Visible = false;
		_messageComponentInstance.Visible = false;
		
		_messageComponentScript = (MessageComponent)_messageComponentInstance;
	}

	public override void _Process(double delta)
	{
		_masterVolumeLabel.Text = $"{Math.Round(_masterVolumeSlider.Value)}%";
		_effectVolumeLabel.Text = $"{Math.Round(_effectVolumeSlider.Value)}%";
		_musicVolumeLabel.Text = $"{Math.Round(_musicVolumeSlider.Value)}%";
	}

	private void OnApplyChangesButtonPressed()
	{
		UserDataManager.Instance.CurrentUser.MasterVolume = (float)_masterVolumeSlider.Value / 100;
		UserDataManager.Instance.CurrentUser.EffectVolume = (float)_effectVolumeSlider.Value / 100;
		UserDataManager.Instance.CurrentUser.MusicVolume = (float)_musicVolumeSlider.Value / 100;

		// Apply changes
		SubmitVolumeChangeToServer();
	}

	private void OnBackButtonPressed()
	{
		SceneManager.Instance.ChangeToMainMenuScene();
	}

	private void SubmitVolumeChangeToServer()
	{
		_loadingComponent.Visible = true;
		string updateVolumeUrl = ApiClient.Instance.BuildUrl($"users/{UserDataManager.Instance.CurrentUser.Id}/volume-settings");

		// prepare headers for the request
		var headers = new string[]
		{
			"Content-Type: application/json",
			$"Authorization: Bearer {UserDataManager.Instance.CurrentUser.AuthToken}"
		};

		// prepare body for the request
		var data = new Godot.Collections.Dictionary
		{
			{"masterVolume", UserDataManager.Instance.CurrentUser.MasterVolume},
			{"effectVolume", UserDataManager.Instance.CurrentUser.EffectVolume},
			{"musicVolume", UserDataManager.Instance.CurrentUser.MusicVolume}
		};
		string body = Json.Stringify(data);
		GD.Print("Submitting volume settings:");
		GD.Print(body);
		// submit request
		_httpRequest.Request(updateVolumeUrl, headers, HttpClient.Method.Put, body);
	}

	private void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		_loadingComponent.Visible = false;

		if (responseCode == 200)
		{
			_messageComponentInstance.Visible = true;
			_messageComponentScript.MessageLabel.Text = "Volume settings updated successfully.";
			_messageComponentScript.ClickButton.Text = "Ok";
			_messageComponentScript.ClickButton.Pressed += OnMessageComponentButtonPressed;
		}
		else
		{
			_messageComponentInstance.Visible = true;
			_messageComponentScript.MessageLabel.Text = "Failed to update volume settings.";
			_messageComponentScript.ClickButton.Text = "Ok";
			_messageComponentScript.ClickButton.Pressed += OnMessageComponentButtonPressed;
		}
	}
	
	private void OnMessageComponentButtonPressed()
	{
		_messageComponentInstance.Visible = false;
	}
}
