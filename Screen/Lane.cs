using Godot;
using System;

public partial class Lane : Node2D
{
	[Export] public string KeyCode { get; set; }
	[Export] public Line2D LaneLine;
	[Export] public TextureRect HittingArea;
	[Export] public Label LaneLabel;

	[Export] public float LaneWidth = 100f;
	[Export] public float HittingAreaHeight = 80f;
	[Export] public float LaneLineWidth = 2f;
	[Export] public int FontSize = 32;

	[Export] public Vector2 StartPoint = new Vector2(0, 0);

	public override void _Ready()
	{
		Vector2 viewportSize = GetViewportRect().Size;
		LaneLine.Width = LaneLineWidth;

		Vector2 LeftEndPoint = new Vector2(0, viewportSize.Y);
		Vector2 LeftHittingBoxPoint = new Vector2(0, viewportSize.Y - HittingAreaHeight);
		Vector2 RightHittingBoxPoint = new Vector2(LaneWidth, viewportSize.Y - HittingAreaHeight);
		Vector2 RightStartPoint = new Vector2(LaneWidth, 0);
		Vector2 RightEndPoint = new Vector2(LaneWidth, viewportSize.Y);

		LaneLine.ClearPoints();
		LaneLine.AddPoint(StartPoint);
		LaneLine.AddPoint(LeftEndPoint);
		LaneLine.AddPoint(LeftHittingBoxPoint);
		LaneLine.AddPoint(RightHittingBoxPoint);
		LaneLine.AddPoint(RightStartPoint);
		LaneLine.AddPoint(RightEndPoint);

		HittingArea.Size = new Vector2(LaneWidth, HittingAreaHeight);
		HittingArea.Position = new Vector2(0, viewportSize.Y - HittingAreaHeight);

		LaneLabel.Position = new Vector2(LaneWidth / 2, viewportSize.Y - HittingAreaHeight / 2);
		LaneLabel.Text = KeyCode;
		LaneLabel.LabelSettings.FontSize = FontSize;
		LaneLabel.LabelSettings.OutlineSize = 2;
	}
}
