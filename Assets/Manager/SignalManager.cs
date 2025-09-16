using Godot;
using System;

public partial class SignalManager : Node
{
    public static SignalManager Instance { get; private set; }

    [Signal]
    public delegate void ResetProgressEventHandler();

    [Signal]
    public delegate void ProgressStartedEventHandler();

    [Signal]
    public delegate void CurrentProgressEndedEventHandler();

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public void EmitResetProgress()
    {
        EmitSignal(SignalName.ResetProgress);
    }

    public void EmitProgressStarted()
    {
        EmitSignal(SignalName.ProgressStarted);
    }

    public void EmitCurrentProgressEnded()
    {
        EmitSignal(SignalName.CurrentProgressEnded);
    }
}
