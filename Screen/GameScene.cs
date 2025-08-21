using Godot;
using System;

public partial class GameScene : Control
{
	[Export] private Node2D _lane1;
	[Export] private Node2D _lane2;
	[Export] private Node2D _lane3;
	[Export] private Node2D _lane4;
	[Export] private Button _pauseButton;

	[Export] private float _laneSpacing = 0f;
	[Export] private float _laneWidth = 100f;

	public override void _Ready()
	{
		InitLanesLocation();
		_pauseButton.Pressed += OnPauseButtonPressed;
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
