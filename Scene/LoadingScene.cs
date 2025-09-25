using Godot;
using System;

public partial class LoadingScene : Control
{
	[Export] private Label _trackCountLabel;

	public override void _Ready()
	{
		ReadyAsync();
	}
	
	private async void ReadyAsync()
	{
		// Update the label with the current progress
		_trackCountLabel.Text = $"Track {GameProgressManger.Instance.CurrentPlayCount + 1}/{GameProgressManger.Instance.TargetPlayCount}";

		await ToSignal(GetTree().CreateTimer(1f), "timeout"); // Optional delay for better UX
		SceneManager.Instance.ChangeToGameScene();
	}
}
