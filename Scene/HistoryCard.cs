using Godot;
using System;

public partial class HistoryCard : Control
{
	private string _songName;
	private int _songId;
	private int _chartId;
	private string _difficulty;
	private float _level;
	private int _trackNo;
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

	public override void _Ready()
	{
		Initialize("testSongName", 1, 1, "ADVANCE", 13.4f, 2, new DateTime(2024, 9, 2, 8, 33, 00), 99.9999f, 101f, "this is a sting");
	}


	public void Initialize(string songName, int songId, int chartId, string difficulty,
		float level, int trackNo, DateTime recordTime, float accuracy, float attentionLevel,
		string imagePath)
	{
		_songName = songName;
		_songId = songId;
		_chartId = chartId;
		_difficulty = difficulty.ToUpper();
		_level = level;
		_trackNo = trackNo;
		_recordTime = recordTime;
		_accuracy = accuracy;
		_attentionLevel = attentionLevel;

		Color thermColor;

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
			default:
				thermColor = GameSetting.Instance.BasicLevelThermColor;
				break;
		}

		// set level label
		_difficultyLabel.Clear();
		_difficultyLabel.PushColor(new Color("#ffffff"));
		_difficultyLabel.PushOutlineColor(thermColor);
		_difficultyLabel.PushOutlineSize(16);
		_difficultyLabel.PushBold();
		_difficultyLabel.AppendText(" " + difficulty);
		_difficultyLabel.PopAll();

		// set track num
		_trackLabel.Text = trackNo.ToString();

		// set name
		_nameLabel.Text = songName;

		// set level
		_levelLabel.Text = level.ToString();

		// set accuracy
		_accuracyLabel.Text = accuracy.ToString() + "%";

		// set record time
		_recordTimeLabel.Text = recordTime.ToString("yyyy/MM/dd hh:mm");

		// set attention level
		_attentionLevelLabel.Text = attentionLevel.ToString();

		var panelStyleBox = new StyleBoxFlat();
		panelStyleBox.CornerRadiusBottomLeft = 10;
		panelStyleBox.CornerRadiusBottomRight = 10;
		panelStyleBox.CornerRadiusTopLeft = 10;
		panelStyleBox.CornerRadiusTopRight = 10;
		panelStyleBox.BgColor = thermColor;

		_backgroundPanel.AddThemeStyleboxOverride("panel", panelStyleBox);

		// add image
		// _imageTextureRect.Texture = GD.Load<Texture2D>(imagePath);

		_detailsButton.Pressed += OnDetailsButtonPressed;
		Visible = true;
	}

	private void OnDetailsButtonPressed()
	{

	}
}
