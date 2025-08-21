using Godot;
using System;

public partial class Lane : Node2D
{
	[Export] public string KeyCode { get; set; }
	[Export] private Line2D _laneLine;
	[Export] private TextureRect _hittingArea;
	[Export] private Label _laneLabel;

	[Export] private float _laneWidth = 100f;
	[Export] private float _hittingAreaHeight = 80f;
	[Export] private float _laneLineWidth = 2f;
	[Export] private int _fontSize = 32;
	[Export] private Vector2 _startPoint = new Vector2(0, 0);
	[Export] private Texture2D _normalTexture;
	[Export] private Texture2D _pressedTexture;
	[Export] private Color _normalHintTextColor;
	[Export] private Color _pressedHintTextColor;

	public override void _Ready()
	{
		Vector2 viewportSize = GetViewportRect().Size;
		_laneLine.Width = _laneLineWidth;

		Vector2 leftEndPoint = new Vector2(0, viewportSize.Y);
		Vector2 leftHittingBoxPoint = new Vector2(0, viewportSize.Y - _hittingAreaHeight);
		Vector2 rightHittingBoxPoint = new Vector2(_laneWidth, viewportSize.Y - _hittingAreaHeight);
		Vector2 rightStartPoint = new Vector2(_laneWidth, 0);
		Vector2 rightEndPoint = new Vector2(_laneWidth, viewportSize.Y);

		_laneLine.ClearPoints();
		_laneLine.AddPoint(_startPoint);
		_laneLine.AddPoint(leftEndPoint);
		_laneLine.AddPoint(leftHittingBoxPoint);
		_laneLine.AddPoint(rightHittingBoxPoint);
		_laneLine.AddPoint(rightStartPoint);
		_laneLine.AddPoint(rightEndPoint);

		_hittingArea.Size = new Vector2(_laneWidth, _hittingAreaHeight);
		_hittingArea.Position = new Vector2(0, viewportSize.Y - _hittingAreaHeight);
		_hittingArea.Texture = _normalTexture;

		_laneLabel.Position = new Vector2(_laneWidth / 2, viewportSize.Y - _hittingAreaHeight / 2);
		_laneLabel.Text = KeyCode;
		_laneLabel.LabelSettings.FontSize = _fontSize;
		_laneLabel.LabelSettings.OutlineSize = 2;
	}

	public void OnKeyPressed()
	{
		_hittingArea.Texture = _pressedTexture;
		_laneLabel.Modulate = _pressedHintTextColor;
	}
	
	public void OnKeyReleased()
	{
		_hittingArea.Texture = _normalTexture;
		_laneLabel.Modulate = _normalHintTextColor;
	}
}
