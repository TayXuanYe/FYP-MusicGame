using Godot;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public partial class ChartManager : Node
{
    private static ChartManager _instance;
    public static ChartManager Instance => _instance;
    public Dictionary<int, string> ChartsIdIndex { get; private set; } = new();
    public Dictionary<float, List<int>> LevelToChartIdsIndex { get; private set; } = new();
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
                            ChartsIdIndex[chartId] = filePath;
                            break;
                        case "Level":
                            float level = float.Parse(value);
                            if (!LevelToChartIdsIndex.ContainsKey(level))
                            {
                                LevelToChartIdsIndex[level] = new List<int>();
                            }
                            LevelToChartIdsIndex[level].Add(chartId);
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

        foreach (string line in File.ReadLines(filePath))
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
        foreach (var format in supportedFormats)
        {
            var musicPath = Path.GetDirectoryName(filePath) + $"/music{format}";
            if (File.Exists(musicPath))
            {
                var musicResource = GD.Load<AudioStream>(musicPath);
                if (musicResource == null)
                {
                    GD.PrintErr($"Failed to load music at path: {musicPath}");
                }
                chartData.music = musicResource;
                break;
            }
        }
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

    public List<int> GetChartIdsByLevel(float level, int amount)
    {
        if (amount <= 0)
        {
            return new List<int>();
        }

        List<int> result = new List<int>();
        HashSet<int> foundChartIds = new HashSet<int>();

        List<float> allLevels = LevelToChartIdsIndex.Keys.ToList();
        allLevels.Sort();
        // use binary search to find the closest level index
        int closestLevelIndex = allLevels.BinarySearch(level);

        // if not found, BinarySearch returns a negative number
        // the bitwise complement of the index of the next element that is larger than value
        if (closestLevelIndex < 0)
        {
            closestLevelIndex = ~closestLevelIndex;
        }

        // set left and right pointers
        int leftIndex = closestLevelIndex;
        int rightIndex = closestLevelIndex + 1;

        // using bfs to find closest levels
        while (result.Count < amount && (leftIndex >= 0 || rightIndex < allLevels.Count))
        {
            GD.Print($"Current result count: {result.Count}, LeftIndex: {leftIndex}, RightIndex: {rightIndex}");
            bool movedLeft = false;
            bool movedRight = false;
            
            // prioritize left side (lower level)
            if (leftIndex >= 0)
            {
                float currentLevel = allLevels[leftIndex];
                List<int> chartIds = LevelToChartIdsIndex[currentLevel];
                foreach (int chartId in chartIds)
                {
                    if (result.Count < amount && foundChartIds.Add(chartId))
                    {
                        result.Add(chartId);
                    }
                }
                leftIndex--;
                movedLeft = true;
            }

            // then check right side (higher level)
            if (result.Count < amount && rightIndex < allLevels.Count)
            {
                float currentLevel = allLevels[rightIndex];
                List<int> chartIds = LevelToChartIdsIndex[currentLevel];
                foreach (int chartId in chartIds)
                {
                    if (result.Count < amount && foundChartIds.Add(chartId))
                    {
                        result.Add(chartId);
                    }
                }
                rightIndex++;
                movedRight = true;
            }

            // if neither side can move, break the loop
            if (!movedLeft && !movedRight)
            {
                break;
            }
        }

        return result;
    }
}

