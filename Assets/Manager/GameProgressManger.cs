using Godot;
using System;
using System.Collections.Generic;

public partial class GameProgressManger : Node
{
    public static GameProgressManger Instance { get; private set; }
    public List<ChartData> CurrentCharts { get; set; } = new();
    private int TargetPlayCount = 4;
    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;

        // get chart data by level
        float level = 5.0f; // example level
        if (level != 0f)
        {
            List<int> chartIds = ChartManager.Instance.GetChartIdsByLevel(level, TargetPlayCount);
            foreach (var chartId in chartIds)
            {
                ChartData chartData = ChartManager.Instance.LoadChart(chartId);
                if (chartData != null)
                {
                    CurrentCharts.Add(chartData);
                }
            }
            TargetPlayCount = CurrentCharts.Count;
        }
    }

    // Signal reset progress
    // Signal progress started
    // Signal current progress ended 
}
