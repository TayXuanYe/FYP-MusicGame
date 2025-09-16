using Godot;
using System;

public partial class SignalManager : Node
{
    public static SignalManager Instance { get; private set; }

    [Signal]
    public delegate void ProgressStartedEventHandler();

    [Signal]
    public delegate void CurrentProgressEndedEventHandler();

    [Signal]
    public delegate void ChartDataLoadedReadyEventHandler();

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public void EmitProgressStarted()
    {
        EmitSignal(SignalName.ProgressStarted);
    }

    public void EmitCurrentProgressEnded()
    {
        EmitSignal(SignalName.CurrentProgressEnded);
    }

    public void EmitChartDataLoadedReady()
    {
        EmitSignal(SignalName.ChartDataLoadedReady);
    }
}
