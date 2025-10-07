using Godot;
using System;
using System.IO;

public partial class HistoryCard : Control
{
	private string _songName;
	private int _songId;
	private int _chartId;
	private string _difficulty;
	private float _level;
	private DateTime _recordTime;
	private float _accuracy;
	private float _attentionLevel;
	private string _imagePath;

	[Export] private RichTextLabel _difficultyLabel;
	[Export] private Label _trackLabel;
	[Export] private Label _nameLabel;
	[Export] private Label _levelLabel;
	[Export] private Label _accuracyLabel;
	[Export] private Label _recordTimeLabel;
	[Export] private Label _attentionLevelLabel;
	[Export] private Button _detailsButton;
	[Export] private Panel _backgroundPanel;
	[Export] private TextureRect _imageTextureRect;
	private int _historyId;

	private bool _detailsConnected = false;

	string[] supportedFormats = { ".jpg", ".jpeg", ".png", ".webp" };
	public void Initialize(int historyId, string songName, int songId, int chartId, string difficulty,
		float level, int trackNo, DateTime recordTime, float accuracy, float attentionLevel,
		string filePath)
	{
		_historyId = historyId;
		_songName = songName;
		_songId = songId;
		_chartId = chartId;
		_difficulty = difficulty?.ToUpper() ?? string.Empty;
		_level = level;
		_recordTime = recordTime;
		_accuracy = accuracy;
		_attentionLevel = attentionLevel;

		// determine therm color
		Color thermColor = GameSetting.Instance.BasicLevelThermColor;
		switch (_difficulty)
		{
			case "BASIC":
				thermColor = GameSetting.Instance.BasicLevelThermColor;
				break;
			case "ADVANCE":
				thermColor = GameSetting.Instance.AdvanceLevelThermColor;
				break;
			case "EXPERT":
				thermColor = GameSetting.Instance.ExpertLevelThermColor;
				break;
			case "MASTER":
				thermColor = GameSetting.Instance.MasterLevelThermColor;
				break;
		}

		// set difficulty label
		_difficultyLabel.Clear();
		_difficultyLabel.PushColor(new Color("#ffffff"));
		_difficultyLabel.PushOutlineColor(thermColor);
		_difficultyLabel.PushOutlineSize(16);
		_difficultyLabel.PushBold();
		_difficultyLabel.AppendText(" " + difficulty);
		_difficultyLabel.PopAll();

		// set other labels
		_trackLabel.Text = trackNo.ToString();
		_nameLabel.Text = songName;
		_levelLabel.Text = level.ToString("F2");
		_accuracyLabel.Text = accuracy.ToString("F2") + "%";
		_recordTimeLabel.Text = recordTime.ToString("yyyy/MM/dd hh:mm");
		_attentionLevelLabel.Text = attentionLevel.ToString("F2");

		var panelStyleBox = new StyleBoxFlat();
		panelStyleBox.CornerRadiusBottomLeft = 10;
		panelStyleBox.CornerRadiusBottomRight = 10;
		panelStyleBox.CornerRadiusTopLeft = 10;
		panelStyleBox.CornerRadiusTopRight = 10;
		panelStyleBox.BgColor = thermColor;
		_backgroundPanel.AddThemeStyleboxOverride("panel", panelStyleBox);

		// load image: try direct path, then common cover names/extensions in same folder, then res:// fallback
		try
		{
			string dirRes = filePath;
			int lastSlash = filePath.LastIndexOf('/');
			if (lastSlash >= 0)
			{
				dirRes = filePath.Substring(0, lastSlash);
			}
			dirRes = dirRes.Replace("\\", "/");
			foreach (var format in supportedFormats)
			{
				var imageResPath = dirRes + $"/cover{format}";

				if (Godot.ResourceLoader.Exists(imageResPath))
				{
					GD.Print("Loading image from: " + imageResPath);
					var imageResource = GD.Load<Texture>(imageResPath);
					if (imageResource != null)
					{
						GD.Print("Image loaded successfully.");
						_imageTextureRect.Texture = imageResource as Texture2D;
						break;
					}
				}
				else
				{
					GD.Print("Image not found at: " + imageResPath);
				}
			}
		}
		catch { }

		// Connect the pressed handler only once per instance
		if (!_detailsConnected)
		{
			_detailsButton.Pressed += OnDetailsButtonPressed;
			_detailsConnected = true;
		}
		Visible = true;
	}

	private void OnDetailsButtonPressed()
	{
		SceneManager.Instance.ChangeToResultScene(_historyId);
	}
}
