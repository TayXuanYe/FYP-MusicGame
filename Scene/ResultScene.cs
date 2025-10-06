using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
public partial class ResultScene : Control
{
	[Export] private Label _titleLabel;
	[Export] private Label _artistLabel;
	[Export] private Label _difficultyLabel;
	[Export] private Label _levelLabel;
	[Export] private Label _scoreLabel;
	[Export] private Label _maxComboLabel;
	[Export] private Label _accuracyLabel;
	[Export] private Label _finalAttentionLabel;
	[Export] private Label _tapCriticalPerfectLabel;
	[Export] private Label _tapPerfectLabel;
	[Export] private Label _tapGreatLabel;
	[Export] private Label _tapGoodLabel;
	[Export] private Label _tapMissLabel;
	[Export] private Label _holdCriticalPerfectLabel;
	[Export] private Label _holdPerfectLabel;
	[Export] private Label _holdGreatLabel;
	[Export] private Label _holdGoodLabel;
	[Export] private Label _holdMissLabel;

	[Export] private Button _backButton;
	[Export] private Button _nextButton;
	[Export] private Button _exitButton;
	[Export] private HttpRequest _httpRequest;
	[Export] private Control _loadingComponent;
	private int _currentIndex = 0;
	private int _totalCount = 0;
	private ChartPlayResult[] _results;

	public override void _Ready()
	{
		_backButton.Pressed += OnBackButtonPressed;
		_nextButton.Pressed += OnNextButtonPressed;
		_exitButton.Pressed += OnExitButtonPressed;
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		_loadingComponent.Visible = false;
	}

	public void Initialize(int historyId)
	{
		_currentIndex = 0;
		_totalCount = 1;
		_backButton.Visible = false;
		_nextButton.Visible = false;
		_results = new ChartPlayResult[1];
		FetchResultFromServer(historyId);
	}

	public void Initialize()
	{
		_currentIndex = 0;
		_totalCount = GameProgressManger.Instance.PlaylistChartsId.Count;
		_backButton.Visible = _totalCount > 1;
		_nextButton.Visible = _totalCount > 1;
		_results = new ChartPlayResult[_totalCount];
		SubmitRequestAnalyzeResult();
	}

	private void FetchResultFromServer(int historyId)
	{
		_loadingComponent.Visible = true;
		string url = ApiClient.Instance.BuildUrl("history/{historyId}");

		// prepare headers for the request
		var headers = new string[] { "Content-Type: application/json" };

		// submit request
		_httpRequest.Request(url, headers, HttpClient.Method.Get, null);
	}

	private void SubmitRequestAnalyzeResult()
	{
		_loadingComponent.Visible = true;
		string url = ApiClient.Instance.BuildUrl("history/analyze");

		// prepare headers for the request
		var headers = new string[] { "Content-Type: application/json" };

		// prepare body for the request
		var bodyDict = new List<object>();
		for (int i = 0; i < _totalCount; i++)
		{
			bodyDict.Add(new Dictionary<string, object>
			{
				{ "chart_id", GameProgressManger.Instance.PlaylistChartsId[i] },
				{ "user_input_data", GameProgressManger.Instance.RawUserInputData.ContainsKey(i + 1) ? GameProgressManger.Instance.RawUserInputData[i + 1] : new List<ProcessResult>() },
				{ "user_gaze_data", GameProgressManger.Instance.RawUserGazeData.ContainsKey(i + 1) ? GameProgressManger.Instance.RawUserGazeData[i + 1] : new List<GazeData>() }
			});
		}
		string bodyJson = JsonSerializer.Serialize(bodyDict);

		// submit request
		_httpRequest.Request(url, headers, HttpClient.Method.Post, bodyJson);
	}

	private void DisplayResult(int index)
	{
		if (_results == null || index < 0 || index >= _results.Length)
		{
			GD.PrintErr("Invalid index or results not loaded.");
			return;
		}
		int chartId = _results[index].ChartId;
		ChartData chartData = ChartManager.Instance.LoadChart(chartId);
		if (chartData == null)
		{
			GD.PrintErr($"Chart data for Chart ID {chartId} not found.");
			return;
		}

		_titleLabel.Text = chartData.Title;
		_artistLabel.Text = chartData.Artist;
		_difficultyLabel.Text = chartData.Difficulty;
		_levelLabel.Text = chartData.Level.ToString("0.00");
		_scoreLabel.Text = _results[index].Score.ToString();
		_maxComboLabel.Text = _results[index].MaxCombo.ToString();
		_accuracyLabel.Text = _results[index].Accuracy.ToString("0.00");
		_finalAttentionLabel.Text = _results[index].FinalAttention.ToString("0.00");

		_tapCriticalPerfectLabel.Text = _results[index].TapCriticalPerfectCount.ToString();
		_tapPerfectLabel.Text = _results[index].TapPerfectCount.ToString();
		_tapGreatLabel.Text = _results[index].TapGreatCount.ToString();
		_tapGoodLabel.Text = _results[index].TapGoodCount.ToString();
		_tapMissLabel.Text = _results[index].TapMissCount.ToString();

		_holdCriticalPerfectLabel.Text = _results[index].HoldCriticalPerfectCount.ToString();
		_holdPerfectLabel.Text = _results[index].HoldPerfectCount.ToString();
		_holdGreatLabel.Text = _results[index].HoldGreatCount.ToString();
		_holdGoodLabel.Text = _results[index].HoldGoodCount.ToString();
		_holdMissLabel.Text = _results[index].HoldMissCount.ToString();
	}

	private void OnBackButtonPressed()
	{
		if (_currentIndex > 0)
		{
			_currentIndex--;
			DisplayResult(_currentIndex);
		}
	}

	private void OnNextButtonPressed()
	{
		if (_currentIndex < GameProgressManger.Instance.PlaylistChartsId.Count - 1)
		{
			_currentIndex++;
			DisplayResult(_currentIndex);
		}
	}

	private void OnExitButtonPressed()
	{
		SceneManager.Instance.ChangeToMainMenuScene();
	}

	private void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		_loadingComponent.Visible = false;

		if (responseCode == 200)
		{
			string jsonResponse = System.Text.Encoding.UTF8.GetString(body);
			GD.Print($"Success Response body: {jsonResponse}");

			var results = JsonSerializer.Deserialize<List<ChartPlayResult>>(jsonResponse);
			if (results != null && results.Count > 0)
			{
				_results = results.ToArray();
				DisplayResult(_currentIndex);
			}
			else
			{
				GD.PrintErr("No results found in the response.");
			}
		}
		else
		{
			string errorResponse = System.Text.Encoding.UTF8.GetString(body);
			GD.PrintErr($"Error Response Code: {responseCode}, Body: {errorResponse}");
		}
	}
}
