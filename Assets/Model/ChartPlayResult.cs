using System;
using System.Collections.Generic;
using Godot;
public class ChartPlayResult
{
    public int ChartId { get; set; }
    public int? HistoryId { get; set; }
    public int Score { get; set; }
    public int MaxCombo { get; set; }
    public float Accuracy { get; set; }
    public float FinalAttention { get; set; }

    public int TapCriticalPerfectCount { get; set; }
    public int TapPerfectCount { get; set; }
    public int TapGreatCount { get; set; }
    public int TapGoodCount { get; set; }
    public int TapMissCount { get; set; }

    public int HoldCriticalPerfectCount { get; set; }
    public int HoldPerfectCount { get; set; }
    public int HoldGreatCount { get; set; }
    public int HoldGoodCount { get; set; }
    public int HoldMissCount { get; set; }

    public List<double> HitTimings { get; set; }
    public DateTime RecordTime { get; set; }
    public int trackNo { get; set; }
}
