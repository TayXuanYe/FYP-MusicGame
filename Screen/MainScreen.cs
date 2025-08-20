using Godot;
using System;

public partial class MainScreen : Control
{
	[Export] private label _usernameLabel;
	[Export] private Button _playGameButton;
	[Export] private Button _gameHistoryButton;
	[Export] private Button _settingButton;
	[Export] private Button _reportBugButton;

	public override void _Ready()
	{
		_playGameButton.Pressed += OnPlayGameButtonPressed;
		_gameHistoryButton.Pressed += OnGameHistoryButtonPressed;
		_settingButton.Pressed += OnSettingButtonPressed;
		_reportBugButton.Pressed += OnReportBugButtonPressed;
        _usernameLabel.Text = UserDataManager.Instance.Username;
	}

	private void OnPlayGameButtonPressed()
	{
		// Handle play game button pressed
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
}
