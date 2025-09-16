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

    public void EmitCurrentProgressEndedSignal()
    {
        EmitSignal(SignalName.CurrentProgressEnded);
    }

    public void EmitChartDataLoadedReadySignal(int currentPlayCount, int targetPlayCount)
    {
        EmitSignal(SignalName.ChartDataLoadedReady, currentPlayCount, targetPlayCount);
    }
}
