using Godot;
using System;

public partial class PauseScene : Control
{
	[Export] private Button _resumeButton;
	[Export] private Button _retryButton;
	[Export] private Button _quitButton;
	
	public override void _Ready()
	{
		_resumeButton.Pressed += OnResumeButtonPressed;
		_retryButton.Pressed += OnRetryButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
	}

	private void OnResumeButtonPressed()
	{
		// Resume the game
		GetTree().Paused = false;
		Visible = false;
		GD.Print("Resume button pressed, game resumed.");
	}

	private void OnRetryButtonPressed()
	{
		// Reload the current scene
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
	
	private void OnQuitButtonPressed()
	{
		// Go back to the main menu scene
		GetTree().Paused = false;
		//SceneManager.Instance.GoToMainMenuScene();
	}
}
