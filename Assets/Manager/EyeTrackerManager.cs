using Godot;
using System;
using Eyeware.BeamEyeTracker;
using System.Runtime.CompilerServices;
public partial class EyeTrackerManager : Node
{
	public static EyeTrackerManager Instance { get; private set; }
	private API _api;
	public bool IsApiConnected { get; private set; } = false;
	private int _connectionAttempts = 0;
	private const int MaxConnectionAttempts = 30;
	
	public override void _Ready()
	{
		GD.Print("EyeTrackerManager is ready.");
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;

		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		Point point00 = new Point(0, 0);
		Point point11 = new Point((Int32)viewportSize.X, (Int32)viewportSize.Y);
		ViewportGeometry viewportGeometry = new ViewportGeometry(point00, point11);

		try
		{
			_api = new API("MUSIC_GAME", viewportGeometry);
			_api.AttemptStartingTheBeamEyeTracker();
			WaitForApiConnectionAsync();
		}
		catch (Exception ex)
		{
			GD.PrintErr("Failed to initialize Eye Tracker API: " + ex.Message);
			// Even if we can't connect to the eye tracker, we should still emit a signal
			// so the game can continue without eye tracking
			SignalManager.Instance.EmitUserEyeTrackingStatusUpdatedSignal(-1, -1, -1);
		}
	}

	private async void WaitForApiConnectionAsync()
	{
		while (!IsApiConnected && _connectionAttempts < MaxConnectionAttempts)
		{
			_connectionAttempts++;
			GD.Print("Waiting for API connection... Attempt " + _connectionAttempts + "/"+ MaxConnectionAttempts + ", Status: " + _api.GetTrackingDataReceptionStatus());
			
			if (_api.GetTrackingDataReceptionStatus() == TrackingDataReceptionStatus.ReceivingTrackingData)
			{
				IsApiConnected = true;
				GD.Print("Eye Tracker API connected successfully!");
				StartGetGazeDataAsync();
				return;
			}
			else if(_api.GetTrackingDataReceptionStatus() == TrackingDataReceptionStatus.NotReceivingTrackingData)
			{
				// try starting the tracker again
				_api.AttemptStartingTheBeamEyeTracker();
			}
			else if(_api.GetTrackingDataReceptionStatus() == TrackingDataReceptionStatus.AttemptingTrackingAutoStart)
			{
				GD.Print("Eye Tracker is attempting to start...");
			}
			
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		}
		
		if (!IsApiConnected)
		{
			GD.PrintErr("Failed to connect to Eye Tracker after " + MaxConnectionAttempts + " attempts.");
			GD.PrintErr("Eye tracking features will be disabled.");
			// Emit a signal to indicate that eye tracking is not available
			SignalManager.Instance.EmitUserEyeTrackingStatusUpdatedSignal(-1, -1, -1);
		}
	}
	
	private async void StartGetGazeDataAsync()
	{
		while (IsApiConnected)
		{
			try
			{
				TrackingStateSet latestStateSet = _api.GetLatestTrackingStateSet();
				UserState userState = latestStateSet.UserState;
				ViewportGaze viewportGazeData = userState.ViewportGaze;
				TrackingConfidence confidence = viewportGazeData.Confidence;

				if (confidence == TrackingConfidence.High || confidence == TrackingConfidence.Medium)
				{
					double x = viewportGazeData.NormalizedPointOfRegard.X;
					double y = viewportGazeData.NormalizedPointOfRegard.Y;
					SignalManager.Instance.EmitUserEyeTrackingStatusUpdatedSignal(x, y, confidence == TrackingConfidence.High ? 1 : 0);
					// GD.Print($"Gaze data: x={x}, y={y}, confidence={confidence}");

					await ToSignal(GetTree().CreateTimer(0.0167f), "timeout");
				}
				else
				{
					// Still connected but low confidence, emit a signal with invalid data
					SignalManager.Instance.EmitUserEyeTrackingStatusUpdatedSignal(-1, -1, -1);
					await ToSignal(GetTree().CreateTimer(0.0167f), "timeout");
				}
			}
			catch (Exception ex)
			{
				GD.PrintErr("Error getting gaze data: " + ex.Message);
				IsApiConnected = false;
				break;
			}
		}
	}
}
