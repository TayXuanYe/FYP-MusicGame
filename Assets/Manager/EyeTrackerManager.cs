using Godot;
using System;
using Eyeware.BeamEyeTracker;
using System.Runtime.CompilerServices;
public partial class EyeTrackerManager : Node
{
    public static EyeTrackerManager Instance { get; private set; }
    private API _api;
    public bool IsApiConnected { get; private set; } = false;
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

        _api = new API("MUSIC_GAME", viewportGeometry);
        _api.AttemptStartingTheBeamEyeTracker();
        WaitForApiConnectionAsync();
    }

    private async void WaitForApiConnectionAsync()
    {
        while (!IsApiConnected)
        {
            GD.Print("Waiting for API connection..." + _api.GetTrackingDataReceptionStatus());
            if (_api.GetTrackingDataReceptionStatus() == TrackingDataReceptionStatus.ReceivingTrackingData)
            {
                IsApiConnected = true;
                StartGetGazeDataAsync();
                break;
            }
            else if(_api.GetTrackingDataReceptionStatus() == TrackingDataReceptionStatus.NotReceivingTrackingData)
            {
                // try starting the tracker again
                _api.AttemptStartingTheBeamEyeTracker();
            }
            await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        }
        GD.Print("API connected!");
    }
    
    private async void StartGetGazeDataAsync()
    {
        while (IsApiConnected)
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
        }
    }
}
