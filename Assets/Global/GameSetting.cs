using Godot;
using System;

public partial class GameSetting : Node
{
	// unit px per second
	[Export] public float NoteSpeed { get; set; } = 800f;

	// should be in config not game setting future enhancement begin
	// unit second
	[Export] public double GoodTimeRange { get; set; } = 0.15f;
	[Export] public double GreatTimeRange { get; set; } = 0.1f;
	[Export] public double PerfectTimeRange { get; set; } = 0.05f;
	[Export] public double CriticalPerfectTimeRange { get; set; } = 0.02f;
	
	[Export] public double GoodHoldDurationRatio { get; set; } = 0.5f;
	[Export] public double GreatHoldDurationRatio { get; set; } = 0.7f;
	[Export] public double PerfectHoldDurationRatio { get; set; } = 0.8f;
	[Export] public double CriticalPerfectHoldDurationRatio { get; set; } = 0.9f;
	// end
	[Export] public Color CriticalPerfectTextColor { get; set; } = Colors.White;
	[Export] public Color PerfectTextColor { get; set; } = Colors.White;
	[Export] public Color GreatTextColor { get; set; } = Colors.White;
	[Export] public Color GoodTextColor { get; set; } = Colors.White;
	[Export] public Color MissTextColor { get; set; } = Colors.White;

	[Export] public Color CriticalPerfectTextOutlineColor { get; set; } = new Color("#ffba00");
	[Export] public Color PerfectTextOutlineColor { get; set; } = new Color("#ff9d03");
	[Export] public Color GreatTextOutlineColor { get; set; } = new Color("#f75ea3");
	[Export] public Color GoodTextOutlineColor { get; set; } = new Color("#2fca4c");
	[Export] public Color MissTextOutlineColor { get; set; } = new Color("#868686");

	[Export] public int HitResultTextOutlineSize { get; set; } = 8;

	[Export] public AudioStream TapSoundEffect { get; set; } = GD.Load<AudioStream>("res://Assets/SoundEffect/SE_Tap.wav");

	private static GameSetting _instance;
	public static GameSetting Instance => _instance;
	public override void _Ready()
	{
		_instance = this;
	}
}
