using System.Collections.Generic;
using Godot;
public class ChartData
{
    public int ChartId { get; set; }
    public int SongId { get; set; }
    public required string Artist { get; set; }
    public required string Difficulty { get; set; }
    public required float Level { get; set; }
    public required string Designer { get; set; }
    public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane1NotesMetadataQueue { get; private set; }
    public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane2NotesMetadataQueue { get; private set; }
    public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane3NotesMetadataQueue { get; private set; }
    public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane4NotesMetadataQueue { get; private set; }
    public string filePath { get; set; }
}