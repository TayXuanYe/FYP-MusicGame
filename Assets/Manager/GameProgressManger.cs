using Godot;
using System;
using System.Collections.Generic;

public partial class GameProgressManger : Node
{
	public static GameProgressManger Instance { get; private set; }
	public List<int> PlaylistChartsId { get; set; } = new();
	public Dictionary<int, List<ProcessResult>> RawUserInputData { get; set; } = new();
	public Dictionary<int, List<GazeData>> RawUserGazeData { get; set; } = new();
	public int TargetPlayCount { get; private set; } = 1;
	public int CurrentPlayCount { get; private set; } = 0;
	private bool _isGameStart = false;
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

	public void AddUserInputData(ProcessResult result)
	{
		if (!_isGameStart) return;

		if (!RawUserInputData.ContainsKey(CurrentPlayCount))
		{
			RawUserInputData[CurrentPlayCount] = new List<ProcessResult>();
		}
		RawUserInputData[CurrentPlayCount].Add(result);
		GD.Print($"Input data recorded: NoteType={result.NoteType}, LaneIndex={result.LaneIndex}, HitResult={result.HitResult}, HitTime={result.HitTime}, TargetHitTime={result.TargetHitTime}, SystemTime={result.SystemTime}");
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

		PlaylistChartsId = ChartManager.Instance.GetChartIdsByDifficulty(UserDataManager.Instance.CurrentUser.SuggestedDifficulty, TargetPlayCount);
		TargetPlayCount = PlaylistChartsId.Count;

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
		// GD.Print($"Gaze data recorded: x={x}, y={y}, confidence={confidence}, timestamp={timestamp}");
	}

	private void OutputRawUserData()
	{
		// output to a directory name Output which is under the project root directory
		string outputDir = "Output";
		if (!System.IO.Directory.Exists(outputDir))
		{
			System.IO.Directory.CreateDirectory(outputDir);
		}

		// output in txt folder and name with timestamp
		string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
		string inputFilePath = System.IO.Path.Combine(outputDir, $"UserInput_{timestamp}.txt");
		string gazeFilePath = System.IO.Path.Combine(outputDir, $"UserGaze_{timestamp}.txt");
		using (var writer = new System.IO.StreamWriter(inputFilePath))
		{
			foreach (var entry in RawUserInputData)
			{
				int chartId = entry.Key;
				foreach (var input in entry.Value)
				{
					if (input.NoteType == "Tap")
					{
						writer.WriteLine($"ChartID: {chartId}, NoteType: {input.NoteType}, LaneIndex: {input.LaneIndex}, HitResult: {input.HitResult}, HitTime: {input.HitTime}, TimeDifference: {input.TimeDifference}, TargetHitTime: {input.TargetHitTime}, SystemTime: {input.SystemTime}");
					}
					else if (input.NoteType == "Hold")
					{
						writer.WriteLine($"ChartID: {chartId}, NoteType: {input.NoteType}, LaneIndex: {input.LaneIndex}, HitResult: {input.HitResult}, HitTime: {input.HitTime}, TargetHitTime: {input.TargetHitTime}, SystemTime: {input.SystemTime}, DurationTime: {input.DurationTime}, HoldTotalTime: {input.HoldTotalTime}, HoldTimeRatio: {input.HoldTimeRatio}");
					}
				}

				writer.WriteLine($"[UserData]");
				// future use
				writer.WriteLine($"Level: {10}");
			}
		}

		using (var writer = new System.IO.StreamWriter(gazeFilePath))
		{
			foreach (var entry in RawUserGazeData)
			{
				int playIndex = entry.Key;
				foreach (var gaze in entry.Value)
				{
					writer.WriteLine($"PlayIndex: {playIndex}, X: {gaze.X}, Y: {gaze.Y}, Confidence: {gaze.Confidence}, Timestamp: {gaze.Timestamp}");
				}
			}
		}
	}
}
