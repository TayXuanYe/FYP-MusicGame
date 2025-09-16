using Godot;
using System;

public partial class MainScene : Control
{
	[Export] private Label _usernameLabel;
	[Export] private Button _playGameButton;
	[Export] private Button _gameHistoryButton;
	[Export] private Button _settingButton;
	[Export] private Button _reportBugButton;
	[Export] private Button _logoutButton;

	public override void _Ready()
	{
		if (!UserDataManager.Instance.CurrentUser.IsLogin)
		{
			SceneManager.Instance.ChangeToLoginScene();
			return;
		}

		_playGameButton.Pressed += OnPlayGameButtonPressed;
		_gameHistoryButton.Pressed += OnGameHistoryButtonPressed;
		_settingButton.Pressed += OnSettingButtonPressed;
		_reportBugButton.Pressed += OnReportBugButtonPressed;
		_logoutButton.Pressed += OnLogoutButtonPressed;
		_usernameLabel.Text = UserDataManager.Instance.CurrentUser.Username;
	}

	private void OnPlayGameButtonPressed()
	{
		SignalManager.Instance.EmitProgressStarted();
	}

	private void OnGameHistoryButtonPressed()
	{
		// Handle game history button pressed
	}

	private void OnSettingButtonPressed()
	{
		// Handle setting button pressed
	}

	private void OnReportBugButtonPressed()
	{
		// Handle report bug button pressed
	}

	private void OnLogoutButtonPressed()
	{
		UserDataManager.Instance.CurrentUser.IsLogin = false;
		SceneManager.Instance.ChangeToLoginScene();
	}
}
