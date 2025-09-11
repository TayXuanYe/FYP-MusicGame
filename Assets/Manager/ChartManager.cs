using Godot;
using System;

public partial class ChartManager : Node
{
    private static ChartManager _instance;
    public static ChartManager Instance => _instance;

    public override void _Ready()
    {
        _instance = this;
    }

}
