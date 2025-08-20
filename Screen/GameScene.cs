using Godot;
using System;

public partial class GameScene : Control
{
	[Export] public Node2D lane1;
	[Export] public Node2D lane2;
	[Export] public Node2D lane3;
	[Export] public Node2D lane4;

	[Export] public float LaneSpacing = 0f;
	[Export] public float LaneWidth = 100f;

	public override void _Ready()
	{
		Vector2 viewportSize = GetViewportRect().Size;
		float centerX = viewportSize.X / 2;

		float totalWidth = (4 * LaneWidth) + (3 * LaneSpacing);

		float startX = centerX - (totalWidth / 2);
		lane1.Position = new Vector2(startX, lane1.Position.Y);

		float lane2X = startX + LaneWidth + LaneSpacing;
		lane2.Position = new Vector2(lane2X, lane2.Position.Y);

		float lane3X = lane2X + LaneWidth + LaneSpacing;
		lane3.Position = new Vector2(lane3X, lane3.Position.Y);
		
		float lane4X = lane3X + LaneWidth + LaneSpacing;
		lane4.Position = new Vector2(lane4X, lane4.Position.Y);
	}
}
