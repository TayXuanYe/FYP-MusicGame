using System.Collections.Generic;
using Godot;

public class ProcessResult
{
    public int LaneIndex { get; set; }
    public string NoteType { get; set; }
    public string HitResult { get; set; }
    public double HitTime { get; set; }
    public double TimeDifference { get; set; }
    public double TargetHitTime { get; set; }
    public double SystemTime { get; set; }
    public double DurationTime { get; set; } // Only for Hold notes
    protected ProcessResult() { }
    public static ProcessResult CreateTapNoteResult(int laneIndex, string hitResult, double hitTime, double timeDifference, double targetHitTime, double systemTime)
    {
        return new ProcessResult
        {
            LaneIndex = laneIndex,
            NoteType = "Tap",
            HitResult = hitResult,
            HitTime = hitTime,
            TimeDifference = timeDifference,
            TargetHitTime = targetHitTime,
            SystemTime = systemTime,
            DurationTime = 0 // Not applicable for Tap notes
        };
    }

    public static ProcessResult CreateHoldNoteResult(int laneIndex, string hitResult, double hitTime, double timeDifference, double targetHitTime, double systemTime, double durationTime)
    {
        return new ProcessResult
        {
            LaneIndex = laneIndex,
            NoteType = "Hold",
            HitResult = hitResult,
            HitTime = hitTime,
            TimeDifference = timeDifference,
            TargetHitTime = targetHitTime,
            SystemTime = systemTime,
            DurationTime = durationTime
        };
    }

}