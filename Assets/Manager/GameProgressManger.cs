using Godot;
using System;
using System.Collections.Generic;

public partial class GameProgressManger : Node
{
	public static GameProgressManger Instance { get; private set; }
	public List<int> PlaylistChartsId { get; set; } = new();
	public Dictionary<int, List<ProcessResult>> RawUserInputData { get; set; } = new();
	public Dictionary<int,List<GazeData>> RawUserGazeData { get; set; } = new();
	public int TargetPlayCount { get; private set; } = 2;
	public int CurrentPlayCount { get; private set; } = 0;
	private bool _isGameStart = false;
	float level = 5.0f; // example level
	public override void _Ready()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
		// Connect to SignalManager signals
		SignalManager.Instance.CurrentProgressEnded += OnCurrentProgressEndSignalReceived;
		SignalManager.Instance.UserEyeTrackingStatusUpdated += OnUserEyeTrackingStatusUpdatedSignalReceived;
	}

	public override void _Process(double delta)
	{
		GD.Print($"Current Play Count: {CurrentPlayCount}, Target Play Count: {TargetPlayCount}, CurrentCharts Count: {PlaylistChartsId.Count}");
	}


	// Signal progress started
	public void StartProgress()
	{
		// reset current progress data
		PlaylistChartsId.Clear();
		RawUserInputData.Clear();
		CurrentPlayCount = 0;
		RawUserGazeData.Clear();
		_isGameStart = true;

		if (level != 0f)
		{
			PlaylistChartsId = ChartManager.Instance.GetChartIdsByLevel(level, TargetPlayCount);
			TargetPlayCount = PlaylistChartsId.Count;
		}

		SceneManager.Instance.ChangeToLoadingScene();
	}

	// Call this method when a chart is completed
	public void OnCurrentProgressEndSignalReceived()
	{
		CurrentPlayCount++;
		if (CurrentPlayCount >= TargetPlayCount)
		{
			_isGameStart = false;
			// Change to result scene
			SceneManager.Instance.ChangeToResultScene();
		}
		else
		{
			SceneManager.Instance.ChangeToLoadingScene();
		}
	}
	
	private void OnUserEyeTrackingStatusUpdatedSignalReceived(double x, double y, int confidence)
	{
		if (!_isGameStart) return;
		
		// Record gaze data with timestamp
		double timestamp = System.Diagnostics.Stopwatch.GetTimestamp() / (double)System.Diagnostics.Stopwatch.Frequency;
		if (!RawUserGazeData.ContainsKey(CurrentPlayCount))
		{
			RawUserGazeData[CurrentPlayCount] = new List<GazeData>();
		}
		RawUserGazeData[CurrentPlayCount].Add(new GazeData(x, y, confidence, timestamp));
		GD.Print($"Gaze data recorded: x={x}, y={y}, confidence={confidence}, timestamp={timestamp}");
	}
}
