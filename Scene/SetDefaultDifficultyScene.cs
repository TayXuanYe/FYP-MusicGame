using Godot;
using System;

public partial class SetDefaultDifficultyScene : Control
{
	[Export] private Button _basicButton;
	[Export] private Button _advancedButton;
	[Export] private Button _expertButton;

	public override void _Ready()
	{
		_basicButton.Pressed += OnBasicButtonPressed;
		_advancedButton.Pressed += OnAdvancedButtonPressed;
		_expertButton.Pressed += OnExpertButtonPressed;
	}

	private void OnBasicButtonPressed()
	{
		UserDataManager.Instance.CurrentUser.SuggestedDifficulty = "Basic";
		SceneManager.Instance.ChangeToMainMenuScene();
	}

	private void OnAdvancedButtonPressed()
	{
		UserDataManager.Instance.CurrentUser.SuggestedDifficulty = "Advanced";
		SceneManager.Instance.ChangeToMainMenuScene();
	}

	private void OnExpertButtonPressed()
	{
		UserDataManager.Instance.CurrentUser.SuggestedDifficulty = "Expert";
		SceneManager.Instance.ChangeToMainMenuScene();
	}
}
