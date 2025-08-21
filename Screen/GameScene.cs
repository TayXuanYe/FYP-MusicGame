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

	[Export] private float _laneSpacing = 0f;
	[Export] private float _laneWidth = 100f;

	public override void _Ready()
	{
		InitBackground();
		InitLanesLocation();
		_pauseButton.Pressed += OnPauseButtonPressed;
	}

public override void _UnhandledInput(InputEvent @event)
{
	if (@event.IsActionPressed("key_D")) { _lane1.OnKeyPressed();}
	if (@event.IsActionPressed("key_F")) { _lane2.OnKeyPressed();}
	if (@event.IsActionPressed("key_J")) { _lane3.OnKeyPressed();}
	if (@event.IsActionPressed("key_K")) { _lane4.OnKeyPressed();}

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
	private void InitLanesLocation()
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
	
	private void OnPauseButtonPressed()
	{
		// Handle pause button pressed logic here
	}
}
