using Godot;
using System;

public partial class LoadingScene : Control
{
	[Export] private Label _trackCountLabel;
	[Export] private Label _songTitleLabel;
	[Export] private Label _difficultyLabel;

	public override void _Ready()
	{
		ReadyAsync();
	}
	
	private async void ReadyAsync()
	{
		// Update the label with the current progress
		_trackCountLabel.Text = $"Track {GameProgressManger.Instance.CurrentPlayCount + 1}/{GameProgressManger.Instance.TargetPlayCount}";

		// Get current chart data
		if (GameProgressManger.Instance.PlaylistChartsId.Count > GameProgressManger.Instance.CurrentPlayCount)
		{
			int currentChartId = GameProgressManger.Instance.PlaylistChartsId[GameProgressManger.Instance.CurrentPlayCount];
			ChartData chartData = ChartManager.Instance.LoadChart(currentChartId);
			
			if (chartData != null)
			{
				// Update song title and difficulty info
				_songTitleLabel.Text = chartData.Title;
				_difficultyLabel.Text = $"{chartData.Difficulty} Lv.{chartData.Level}";
			}
		}

		await ToSignal(GetTree().CreateTimer(1f), "timeout"); // Optional delay for better UX
		SceneManager.Instance.ChangeToGameScene();
	}
}
