using Godot;
using System;

public partial class SignalManager : Node
{
    public static SignalManager Instance { get; private set; }

    [Signal]
    public delegate void CurrentProgressEndedEventHandler();

    [Signal]
    public delegate void ChartDataLoadedReadyEventHandler(int currentPlayCount, int targetPlayCount);

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public void EmitCurrentProgressEnded()
    {
        EmitSignal(SignalName.CurrentProgressEnded);
    }

    public void EmitChartDataLoadedReady(int currentPlayCount, int targetPlayCount)
    {
        EmitSignal(SignalName.ChartDataLoadedReady, currentPlayCount, targetPlayCount);
    }
}
