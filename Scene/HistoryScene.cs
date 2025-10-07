using Godot;
using System;
using System.Collections.Generic;

public partial class HistoryScene : Control
{
	[Export] private HttpRequest _httpRequest;
	[Export] private VBoxContainer _historyList;
	[Export] private PackedScene _historyCardComponent;
	[Export] private Control _loadingComponent;
	

	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnRequestCompleted;
		LoadHistory();
	}

	private void LoadHistory()
	{
		_loadingComponent.Visible = true;
		string url = ApiClient.Instance.BuildUrl($"history/user/{UserDataManager.Instance.CurrentUser.Id}");
		_httpRequest.Request(url);
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		if (responseCode == 200)
		{
			string jsonResponse = System.Text.Encoding.UTF8.GetString(body);
			GD.Print("History data received: " + jsonResponse);

			var parsed = Json.ParseString(jsonResponse);
			List<ChartPlayResult> historyData = new List<ChartPlayResult>();
			if (parsed.VariantType == Variant.Type.Array)
			{
					foreach (var item in parsed.AsGodotArray())
					{
						var dict = (Godot.Collections.Dictionary)item;
						var chartPlayResult = new ChartPlayResult();
						chartPlayResult.ChartId = (int)dict["chart_id"];
						chartPlayResult.HistoryId = (int)dict["id"];
						chartPlayResult.Score = (int)dict["score"];
						chartPlayResult.MaxCombo = (int)dict["max_combo"];
						chartPlayResult.Accuracy = (float)dict["accuracy"];
						chartPlayResult.FinalAttention = (float)dict["final_attention"];
						chartPlayResult.TapCriticalPerfectCount = (int)dict["tap_critical_perfect_count"];
						chartPlayResult.TapPerfectCount = (int)dict["tap_perfect_count"];
						chartPlayResult.TapGreatCount = (int)dict["tap_great_count"];
						chartPlayResult.TapGoodCount = (int)dict["tap_good_count"];
						chartPlayResult.TapMissCount = (int)dict["tap_miss_count"];
						chartPlayResult.HoldCriticalPerfectCount = (int)dict["hold_critical_perfect_count"];
						chartPlayResult.HoldPerfectCount = (int)dict["hold_perfect_count"];
						chartPlayResult.HoldGreatCount = (int)dict["hold_great_count"];
						chartPlayResult.HoldGoodCount = (int)dict["hold_good_count"];
						chartPlayResult.HoldMissCount = (int)dict["hold_miss_count"];
						chartPlayResult.HitTimings = new List<double>();
						foreach (var timing in ((Godot.Collections.Array)dict["hit_timings"]))
						{
							chartPlayResult.HitTimings.Add((double)timing);
						}
						historyData.Add(chartPlayResult);                
				}
			}
			_loadingComponent.Visible = false;
			foreach (var item in historyData)
			{
				var historyCard = _historyCardComponent.Instantiate<HistoryCard>();
				ChartData chartData = ChartManager.Instance.LoadChart(item.ChartId);
				string imagePath = chartData.filePath.Replace("data.txt", "cover.png");
				historyCard.Initialize(item.HistoryId ?? 0, chartData.Title, chartData.SongId, item.ChartId, chartData.Difficulty,
					chartData.Level, item.trackNo, item.RecordTime, item.Accuracy, item.FinalAttention,
					imagePath);
				_historyList.AddChild(historyCard);
			}
		}
		else
		{
			GD.PrintErr("Failed to load history data. Response code: " + responseCode);
		}
	}
}
