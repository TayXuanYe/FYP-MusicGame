using Godot;
using System;

public partial class GameScene : Node2D
{
	[Export] private Lane _lane1;
	[Export] private Lane _lane2;
	[Export] private Lane _lane3;
	[Export] private Lane _lane4;
	[Export] private Button _pauseButton;
	[Export] private TextureRect _background;
	[Export] private Control _uiContainer;
	[Export] private Panel _navBarPanel;
	[Export] private RichTextLabel _hitResultDisplayRichTextLabel;
	[Export] private float _laneSpacing = 0f;
	[Export] private float _laneWidth = 100f;
	[Export] private PauseScene _pauseScene;

	private Timer _displayTimer;

	public override void _Ready()
	{
		InitBackground();
		InitLanesLayout();
		InitUI();
		_pauseButton.Pressed += OnPauseButtonPressed;
		_lane1.Connect(Lane.SignalName.DisplayResult, new Callable(this, nameof(OnDisplayHiteResultSignalHandler)));
		_lane2.Connect(Lane.SignalName.DisplayResult, new Callable(this, nameof(OnDisplayHiteResultSignalHandler)));
		_lane3.Connect(Lane.SignalName.DisplayResult, new Callable(this, nameof(OnDisplayHiteResultSignalHandler)));
		_lane4.Connect(Lane.SignalName.DisplayResult, new Callable(this, nameof(OnDisplayHiteResultSignalHandler)));

		_displayTimer = new Timer();
		_displayTimer.OneShot = true;
		_displayTimer.WaitTime = 0.5f;
		_displayTimer.Timeout += OnDisplayTimerTimeout;
		AddChild(_displayTimer);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("key_D")) { _lane1.OnKeyPressed(); }
		if (@event.IsActionPressed("key_F")) { _lane2.OnKeyPressed(); }
		if (@event.IsActionPressed("key_J")) { _lane3.OnKeyPressed(); }
		if (@event.IsActionPressed("key_K")) { _lane4.OnKeyPressed(); }

		if (@event.IsActionReleased("key_D")) { _lane1.OnKeyReleased(); }
		if (@event.IsActionReleased("key_F")) { _lane2.OnKeyReleased(); }
		if (@event.IsActionReleased("key_J")) { _lane3.OnKeyReleased(); }
		if (@event.IsActionReleased("key_K")) { _lane4.OnKeyReleased(); }
	}

	private void InitBackground()
	{
		Vector2 viewportSize = GetViewportRect().Size;
		_background.Size = viewportSize;
		_background.Position = Vector2.Zero;
	}
	private void InitLanesLayout()
	{
		Vector2 viewportSize = GetViewportRect().Size;
		float centerX = viewportSize.X / 2;

		float totalWidth = (4 * _laneWidth) + (3 * _laneSpacing);

		float startX = centerX - (totalWidth / 2);
		_lane1.Position = new Vector2(startX, _lane1.Position.Y);

		float lane2X = startX + _laneWidth + _laneSpacing;
		_lane2.Position = new Vector2(lane2X, _lane2.Position.Y);

		float lane3X = lane2X + _laneWidth + _laneSpacing;
		_lane3.Position = new Vector2(lane3X, _lane3.Position.Y);

		float lane4X = lane3X + _laneWidth + _laneSpacing;
		_lane4.Position = new Vector2(lane4X, _lane4.Position.Y);
	}
	private void InitUI()
	{
		Vector2 viewportSize = GetViewportRect().Size;
		_uiContainer.Size = viewportSize;

		_hitResultDisplayRichTextLabel.Position = new Vector2((viewportSize.X - _hitResultDisplayRichTextLabel.Size.X) / 2, viewportSize.Y - 150);
	}
	private void OnPauseButtonPressed()
	{
		_pauseScene.Visible = true;
		GetTree().Paused = true;
	}

	public void DisplayHitResult(string result)
	{
		Color textColor;
		Color outlineColor;

		switch (result)
		{
			case "Critical Perfect":
				textColor = GameSetting.Instance.CriticalPerfectTextColor;
				outlineColor = GameSetting.Instance.CriticalPerfectTextOutlineColor;
				break;
			case "Perfect":
				textColor = GameSetting.Instance.PerfectTextColor;
				outlineColor = GameSetting.Instance.PerfectTextOutlineColor;
				break;
			case "Great":
				textColor = GameSetting.Instance.GreatTextColor;
				outlineColor = GameSetting.Instance.GreatTextOutlineColor;
				break;
			case "Good":
				textColor = GameSetting.Instance.GoodTextColor;
				outlineColor = GameSetting.Instance.GoodTextOutlineColor;
				break;
			case "Miss":
				textColor = GameSetting.Instance.MissTextColor;
				outlineColor = GameSetting.Instance.MissTextOutlineColor;
				break;
			default:
				return;
		}

		_hitResultDisplayRichTextLabel.Clear();
		_hitResultDisplayRichTextLabel.PushColor(textColor);
		_hitResultDisplayRichTextLabel.PushOutlineColor(outlineColor);
		_hitResultDisplayRichTextLabel.PushOutlineSize(GameSetting.Instance.HitResultTextOutlineSize);
		_hitResultDisplayRichTextLabel.AppendText(result);
		_hitResultDisplayRichTextLabel.PopAll();

		_hitResultDisplayRichTextLabel.Visible = true;
		_displayTimer.Start();
	}

	private void OnDisplayHiteResultSignalHandler(string result)
	{
		DisplayHitResult(result);
	}
	
	private void OnDisplayTimerTimeout()
	{
		_hitResultDisplayRichTextLabel.Visible = false;
	}
}
