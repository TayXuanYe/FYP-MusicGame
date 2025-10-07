using Godot;
using System;
using System.Collections.Generic;

public partial class HistoryScene : Control
{
	[Export] private HttpRequest _httpRequest;
	[Export] private VBoxContainer _historyList;
	[Export] private PackedScene _historyCardComponent;
	[Export] private Control _loadingComponent;
	[Export] private MarginContainer _noHistoryContainer;
	[Export] private Button _backButton;

	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnRequestCompleted;
		_backButton.Pressed += OnBackButtonPressed;
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
		bool hasHistory = responseCode == 200 && body.Length > 0;
		_noHistoryContainer.Visible = !hasHistory;
		if (responseCode == 200)
		{
			string jsonResponse = System.Text.Encoding.UTF8.GetString(body);
			GD.Print("History data received: " + jsonResponse);

			var parsed = Json.ParseString(jsonResponse);
			GD.Print(jsonResponse);
			List<ChartPlayResult> historyData = new List<ChartPlayResult>();
			if (parsed.VariantType == Variant.Type.Array)
			{
				foreach (var item in parsed.AsGodotArray())
				{
					var dict = (Godot.Collections.Dictionary)item;
					var chartPlayResult = new ChartPlayResult();
					// typed helpers to extract values directly from Godot dictionary/variants
					int GetInt(Godot.Collections.Dictionary d, string snakeKey, string camelKey, int defaultValue = 0)
					{
						if (d.ContainsKey(snakeKey)) return VariantToInt(d[snakeKey], defaultValue);
						if (d.ContainsKey(camelKey)) return VariantToInt(d[camelKey], defaultValue);
						return defaultValue;
					}

					float GetFloat(Godot.Collections.Dictionary d, string snakeKey, string camelKey, float defaultValue = 0f)
					{
						if (d.ContainsKey(snakeKey)) return VariantToFloat(d[snakeKey], defaultValue);
						if (d.ContainsKey(camelKey)) return VariantToFloat(d[camelKey], defaultValue);
						return defaultValue;
					}

					Godot.Collections.Array GetArray(Godot.Collections.Dictionary d, string snakeKey, string camelKey)
					{
						object v = null;
						if (d.ContainsKey(snakeKey)) v = d[snakeKey];
						else if (d.ContainsKey(camelKey)) v = d[camelKey];
						if (v == null) return null;
						if (v is Godot.Collections.Array arr) return arr;
						if (v is Godot.Variant gv)
						{
							try { return (Godot.Collections.Array)gv; } catch { }
						}
						return null;
					}

					int VariantToInt(object val, int defaultValue = 0)
					{
						if (val == null) return defaultValue;
						try
						{
							if (val is int i) return i;
							if (val is long l) return (int)l;
							if (val is double d) return (int)d;
							if (val is float f) return (int)f;
							if (val is string s && int.TryParse(s, out var r)) return r;
							if (val is Godot.Variant gv)
							{
								var s2 = gv.ToString();
								if (int.TryParse(s2, out var r2)) return r2;
								if (double.TryParse(s2, out var rd)) return (int)rd;
							}
						}
						catch { }
						return defaultValue;
					}

					float VariantToFloat(object val, float defaultValue = 0f)
					{
						if (val == null) return defaultValue;
						try
						{
							if (val is float f) return f;
							if (val is double d) return (float)d;
							if (val is int i) return i;
							if (val is long l) return l;
							if (val is string s && float.TryParse(s, out var r)) return r;
							if (val is Godot.Variant gv)
							{
								var s2 = gv.ToString();
								if (float.TryParse(s2, out var r2)) return r2;
								if (double.TryParse(s2, out var rd)) return (float)rd;
							}
						}
						catch { }
						return defaultValue;
					}

					// Safely extract values with fallback defaults using typed getters
					chartPlayResult.ChartId = GetInt(dict, "chart_id", "chartId", 0);
					chartPlayResult.HistoryId = GetInt(dict, "history_id", "historyId", 0);
					chartPlayResult.Score = GetInt(dict, "score", "score", 0);
					chartPlayResult.MaxCombo = GetInt(dict, "max_combo", "maxCombo", 0);
					chartPlayResult.Accuracy = GetFloat(dict, "accuracy", "accuracy", 0f);
					chartPlayResult.FinalAttention = GetFloat(dict, "final_attention", "finalAttention", 0f);
					chartPlayResult.TapCriticalPerfectCount = GetInt(dict, "tap_critical_perfect_count", "tapCriticalPerfectCount", 0);
					chartPlayResult.TapPerfectCount = GetInt(dict, "tap_perfect_count", "tapPerfectCount", 0);
					chartPlayResult.TapGreatCount = GetInt(dict, "tap_great_count", "tapGreatCount", 0);
					chartPlayResult.TapGoodCount = GetInt(dict, "tap_good_count", "tapGoodCount", 0);
					chartPlayResult.TapMissCount = GetInt(dict, "tap_miss_count", "tapMissCount", 0);
					chartPlayResult.HoldCriticalPerfectCount = GetInt(dict, "hold_critical_perfect_count", "holdCriticalPerfectCount", 0);
					chartPlayResult.HoldPerfectCount = GetInt(dict, "hold_perfect_count", "holdPerfectCount", 0);
					chartPlayResult.HoldGreatCount = GetInt(dict, "hold_great_count", "holdGreatCount", 0);
					chartPlayResult.HoldGoodCount = GetInt(dict, "hold_good_count", "holdGoodCount", 0);
					chartPlayResult.HoldMissCount = GetInt(dict, "hold_miss_count", "holdMissCount", 0);
					chartPlayResult.TrackNo = GetInt(dict, "track_no", "trackNo", 0);
					string GetString(Godot.Collections.Dictionary d, string snakeKey, string camelKey, string defaultValue = null)
					{
						if (d.ContainsKey(snakeKey)) return VariantToString(d[snakeKey], defaultValue);
						if (d.ContainsKey(camelKey)) return VariantToString(d[camelKey], defaultValue);
						return defaultValue;
					}

					string VariantToString(object val, string defaultValue = null)
					{
						if (val == null) return defaultValue;
						try
						{
							if (val is string s) return s;
							if (val is Godot.Variant gv) return gv.ToString();
							return val.ToString();
						}
						catch { }
						return defaultValue;
					}

					string recordTimeStr = GetString(dict, "record_time", "recordTime", null);
					chartPlayResult.RecordTime = recordTimeStr != null ? DateTime.Parse(recordTimeStr) : DateTime.MinValue;

					chartPlayResult.HitTimings = new List<double>();
					var hitArray2 = GetArray(dict, "hit_timings", "hitTimings");
					if (hitArray2 != null)
					{
						foreach (var timing in hitArray2)
						{
							double t = 0;
							try
							{
								// Convert Variant or value to string then parse using InvariantCulture
								string s = null;
								try { s = timing.ToString(); } catch { s = null; }
								if (!string.IsNullOrEmpty(s))
								{
									System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InvariantCulture;
									if (double.TryParse(s, System.Globalization.NumberStyles.Any, ci, out var pr)) t = pr;
								}
							}
							catch { }
							chartPlayResult.HitTimings.Add(t);
						}
					}

					historyData.Add(chartPlayResult);
				}
			}
			_loadingComponent.Visible = false;
			foreach (var item in historyData)
			{
				var historyCard = _historyCardComponent.Instantiate<HistoryCard>();
				ChartData chartData = ChartManager.Instance.LoadChart(item.ChartId);
				string imagePath = chartData.filePath;
				historyCard.Initialize(item.HistoryId ?? 0, chartData.Title, chartData.SongId, item.ChartId, chartData.Difficulty,
					chartData.Level, item.TrackNo, item.RecordTime, item.Accuracy, item.FinalAttention,
					imagePath);
				_historyList.AddChild(historyCard);
			}	
		}
		else
		{
			GD.PrintErr("Failed to load history data. Response code: " + responseCode);
		}
	}

	private void OnBackButtonPressed()
	{
		SceneManager.Instance.ChangeToMainMenuScene();
	}
}
