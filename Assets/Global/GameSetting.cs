using Godot;
using System;

public partial class GameSetting : Node
{
    // unit px per second
    [Export] public float NoteSpeed { get; set; } = 600f;
    // unit second
    [Export] public double GoodTimeRange { get; set; } = 0.15f;
    [Export] public double GreatTimeRange { get; set; } = 0.1f;
    [Export] public double PerfectTimeRange { get; set; } = 0.05f;
    [Export] public double CriticalPerfectTimeRange { get; set; } = 0.02f;

    private static GameSetting _instance;
    public static GameSetting Instance => _instance;
    public override void _Ready()
    {
        _instance = this;
    }
}