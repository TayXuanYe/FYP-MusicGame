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
	[Export] private TextureRect _messageTextureRect;
	[Export] private Label _messageLabel;

	public override void _Ready()
	{
		//if (!UserDataManager.Instance.CurrentUser.IsLogin)
		//{
		//SceneManager.Instance.ChangeToLoginScene();
		//return;
		//}

		if (UserDataManager.Instance.CurrentUser.SuggestedDifficulty == null)
		{
			SceneManager.Instance.ChangeToSetDefaultDifficultyScene();
			return;
		}

		// Initialize eye tracker connection check
		_messageLabel.Text = "Connecting to Eye Tracker...";
		CheckEyeTrackerConnection();

		_playGameButton.Pressed += OnPlayGameButtonPressed;
		_gameHistoryButton.Pressed += OnGameHistoryButtonPressed;
		_settingButton.Pressed += OnSettingButtonPressed;
		_reportBugButton.Pressed += OnReportBugButtonPressed;
		_logoutButton.Pressed += OnLogoutButtonPressed;
		_usernameLabel.Text = UserDataManager.Instance.CurrentUser.Username;

		// Connect to eye tracker status signal
		SignalManager.Instance.UserEyeTrackingStatusUpdated += OnUserEyeTrackingStatusUpdated;
	}

	private void OnPlayGameButtonPressed()
	{
		GameProgressManger.Instance.StartProgress();
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
	
	private async void CheckEyeTrackerConnection()
	{
		// Check connection status periodically
		while (_messageTextureRect.Visible)
		{
			if (EyeTrackerManager.Instance.IsApiConnected)
			{
				// Eye tracker is connected, hide the message
				_messageTextureRect.Visible = false;
				_messageLabel.Text = "Eye Tracker connected successfully!";
				break;
			}
			else
			{
				// Still connecting, update the message
				_messageLabel.Text = "Connecting to Eye Tracker...";
			}
			
			// Wait for 1 second before checking again
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		}
	}

	private void OnUserEyeTrackingStatusUpdated(double x, double y, int confidence)
	{
		// If we receive eye tracking data, it means the eye tracker is connected
		if (_messageTextureRect.Visible)
		{
			_messageTextureRect.Visible = false;
			_messageLabel.Text = "Eye Tracker connected successfully!";
		}
		SignalManager.Instance.UserEyeTrackingStatusUpdated -= OnUserEyeTrackingStatusUpdated;
	}
}
