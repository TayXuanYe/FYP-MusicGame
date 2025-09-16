using System.Collections.Generic;
using Godot;
public class ChartData
{
	public int ChartId { get; set; }
	public int SongId { get; set; }
	public string Artist { get; set; }
	public string Difficulty { get; set; }
	public float Level { get; set; }
	public string Designer { get; set; }
	public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane1NotesMetadataQueue { get; set; }
	public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane2NotesMetadataQueue { get; set; }
	public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane3NotesMetadataQueue { get; set; }
	public Queue<(double targetHitTime, Color noteColor, string type, double durationTime)> Lane4NotesMetadataQueue { get; set; }
	public string filePath { get; set; }
}
