using Godot;
using System;
public partial class SignalManager : Node
{
	public static SignalManager Instance { get; private set; }

	[Signal]
	public delegate void CurrentProgressEndedEventHandler();

	[Signal]
	public delegate void ChartDataLoadedReadyEventHandler(int currentPlayCount, int targetPlayCount);

	[Signal]
	public delegate void UserEyeTrackingStatusUpdatedEventHandler(double x, double y, int confidence);

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

	public void EmitUserEyeTrackingStatusUpdatedSignal(double x, double y, int confidence)
	{
		EmitSignal(SignalName.UserEyeTrackingStatusUpdated, x, y, confidence);
	}
}
