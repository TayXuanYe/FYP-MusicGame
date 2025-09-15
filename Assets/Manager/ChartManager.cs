using Godot;
using System.IO;
using System.Collections.Generic;

public partial class ChartManager : Node
{
    private static ChartManager _instance;
    public static ChartManager Instance => _instance;
    public Dictionary<int, string> ChartsIdIndex { get; private set; } = new();

    public override void _Ready()
    {
        _instance = this;
        LoadChartIndex();
    }

    // load chart data from Charts folder
    private void LoadChartIndex()
    {
        string relativePath = "res://Charts/";
        string rootDirectory = ProjectSettings.GlobalizePath(relativePath);

         if (Directory.Exists(rootDirectory))
        {
            foreach (string filePath in Directory.EnumerateFiles(rootDirectory, "data.txt", SearchOption.AllDirectories))
            {
                var chartsData = ParseDataSection(filePath, "Data");
                foreach (string line in chartsData)
                {
                    string[] parts = line.Split(':');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    switch (key)
                    {
                        case "ChartId":
                            int chartId = int.Parse(value);
                            ChartsIdIndex[chartId] = filePath;
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

    private ChartData LoadChartData(int filePath)
    {

        return null;
    }
}
