using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public partial class ChartManager : Node
{
    private static ChartManager _instance;
    public static ChartManager Instance => _instance;
    // We store resource paths (res://) rather than absolute filesystem paths.
    public Dictionary<int, string> ChartsIdIndex { get; private set; } = new();
    public Dictionary<string, List<int>> DifficultyToChartIdsIndex { get; private set; } = new();
    public override void _Ready()
    {
        _instance = this;
        LoadChartIndex();
    }

    // load chart data from Charts folder
    private void LoadChartIndex()
    {
        GD.Print("Loading chart index...");
        string relativePath = "res://Charts/";
        string rootDirectory = ProjectSettings.GlobalizePath(relativePath);

        if (Directory.Exists(rootDirectory))
        {
            foreach (string filePath in Directory.EnumerateFiles(rootDirectory, "data.txt", SearchOption.AllDirectories))
            {
                GD.Print($"Loading chart data from: {filePath}");
                var chartsData = ParseDataSection(filePath, "Data");
                int chartId = -1;
                foreach (string line in chartsData)
                {
                    string[] parts = line.Split(':');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    GD.Print($"Parsed line - Key: {key}, Value: {value}");
                    switch (key)
                    {
                        case "ChartId":
                            chartId = int.Parse(value);
                            // convert absolute filesystem path to res:// path relative to res://Charts/
                            try
                            {
                                string rel = filePath.StartsWith(rootDirectory) ? filePath.Substring(rootDirectory.Length) : Path.GetFileName(filePath);
                                rel = rel.Replace("\\", "/");
                                ChartsIdIndex[chartId] = relativePath + rel; // store res:// path
                            }
                            catch
                            {
                                // fallback: store full res path using filename only
                                ChartsIdIndex[chartId] = relativePath + Path.GetFileName(filePath);
                            }
                            break;
                        case "Difficulty":
                            string level = value;
                            if (!DifficultyToChartIdsIndex.ContainsKey(level))
                            {
                                DifficultyToChartIdsIndex[level] = new List<int>();
                            }
                            DifficultyToChartIdsIndex[level].Add(chartId);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        else
        {
            GD.PrintErr($"The directory '{rootDirectory}' does not exist.");
        }
    }

    private List<string> ParseDataSection(string filePath, string sectionName)
    {
        List<string> sectionLines = new List<string>();
        bool inTargetSection = false;

        // filePath is stored as a res:// path in ChartsIdIndex. Convert to filesystem path for reading.
        string fsPath = filePath;
        try
        {
            if (filePath.StartsWith("res://")) fsPath = ProjectSettings.GlobalizePath(filePath);
        }
        catch { }

        foreach (string line in File.ReadLines(fsPath))
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                string currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);

                inTargetSection = (currentSection == sectionName);

                // If we've just entered the target section, skip the section header line
                if (inTargetSection)
                {
                    continue; // Skip the header line
                }
            }

            // If we're in the target section, add the line to the list
            if (inTargetSection && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                sectionLines.Add(trimmedLine);
            }
        }

        return sectionLines;
    }

    string[] supportedFormats = { ".ogg", ".wav", ".mp3" };
    public ChartData LoadChart(int chartId)
    {
        if (!ChartsIdIndex.ContainsKey(chartId))
        {
            GD.PrintErr($"Chart ID {chartId} not found in index.");
            return null;
        }

        string filePath = ChartsIdIndex[chartId];
        var chartData = new ChartData();
        chartData.ChartId = chartId;
        chartData.filePath = filePath;

        var dataLines = ParseDataSection(filePath, "Data");
        foreach (string line in dataLines)
        {
            string[] parts = line.Split(':');
            string key = parts[0].Trim();
            string value = parts[1].Trim();
            switch (key)
            {
                case "SongId":
                    chartData.SongId = int.Parse(value);
                    break;
                case "Title":
                    chartData.Title = value;
                    break;
                case "Artist":
                    chartData.Artist = value;
                    break;
                case "Difficulty":
                    chartData.Difficulty = value;
                    break;
                case "Level":
                    chartData.Level = float.Parse(value);
                    break;
                case "Designer":
                    chartData.Designer = value;
                    break;
                default:
                    break;
            }
        }

        // Initialize note queues
        chartData.Lane1NotesMetadataQueue = new Queue<(double, Color, string, double)>();
        chartData.Lane2NotesMetadataQueue = new Queue<(double, Color, string, double)>();
        chartData.Lane3NotesMetadataQueue = new Queue<(double, Color, string, double)>();
        chartData.Lane4NotesMetadataQueue = new Queue<(double, Color, string, double)>();

    LoadLaneNotes(filePath, "Lane1", chartData.Lane1NotesMetadataQueue);
    LoadLaneNotes(filePath, "Lane2", chartData.Lane2NotesMetadataQueue);
    LoadLaneNotes(filePath, "Lane3", chartData.Lane3NotesMetadataQueue);
    LoadLaneNotes(filePath, "Lane4", chartData.Lane4NotesMetadataQueue);

        // load music
        // try loading music using res:// paths — construct directory from res:// path safely
        try
        {
            string dirRes = filePath;
            int lastSlash = filePath.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                dirRes = filePath.Substring(0, lastSlash);
            }
            dirRes = dirRes.Replace("\\", "/");
            foreach (var format in supportedFormats)
            {
                var musicResPath = dirRes + $"/music{format}";

                if (Godot.ResourceLoader.Exists(musicResPath))
                {
                    var musicResource = GD.Load<AudioStream>(musicResPath); 
                    
                    if (musicResource != null)
                    {
                        chartData.music = musicResource;
                        break;
                    }
                }
            }
        }
        catch { }
        return chartData;
    }

    private void LoadLaneNotes(string filePath, string laneSection, Queue<(double, Color, string, double)> laneQueue)
    {
        var notesLines = ParseDataSection(filePath, laneSection);
        foreach (string line in notesLines)
        {
            string[] parts = line.Split(',');
            if (parts.Length < 4)
            {
                GD.PrintErr($"Invalid note format in {laneSection}: {line}");
                continue;
            }

            double targetHitTime = double.Parse(parts[0].Trim());
            string type = parts[1].Trim();
            Color noteColor = new Color(parts[2].Trim());
            double durationTime = double.Parse(parts[3].Trim());

            var noteMetadata = (targetHitTime, noteColor, type, durationTime);
            laneQueue.Enqueue(noteMetadata);
        }
    }

    public List<int> GetChartIdsByDifficulty(string difficulties, int amount)
    {
        if (amount <= 0)
        {
            return new List<int>();
        }

        List<int> result = new List<int>();

        List<int> difficultiesToChartIdsIndexInList = DifficultyToChartIdsIndex.ContainsKey(difficulties) ? DifficultyToChartIdsIndex[difficulties] : new List<int>();
        if (difficultiesToChartIdsIndexInList.Count == 0)
        {
            GD.PrintErr("No difficulties available in the index.");
            return result;
        }
        Random random = new Random();
        List<int> selectedRandomNumbers = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            int randomIndex = random.Next(difficultiesToChartIdsIndexInList.Count);
            if (selectedRandomNumbers.Contains(randomIndex))
            {
                i--;
                continue;
            }
            selectedRandomNumbers.Add(randomIndex);
            int chartId = difficultiesToChartIdsIndexInList[randomIndex];
            result.Add(chartId);
        }

        return result;
    }
}

