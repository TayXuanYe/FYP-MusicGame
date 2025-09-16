using Godot;
using System;
using System.Collections.Generic;

public partial class GameProgressManger : Node
{
    public static GameProgressManger Instance { get; private set; }
    public List<ChartData> CurrentCharts { get; set; } = new();
    public int TargetPlayCount { get; private set; } = 4;
    public int CurrentPlayCount { get; private set; } = 0;
    float level = 5.0f; // example level
    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
        // Connect to SignalManager signals
        SignalManager.Instance.ProgressStarted += OnProgressStartedSignalReceived;
        SignalManager.Instance.CurrentProgressEnded += OnCurrentProgressEndSignalReceived;
    }

    // Signal progress started
    private void OnProgressStartedSignalReceived()
    {
        // init current play count
        CurrentPlayCount = 0;

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

        SceneManager.Instance.ChangeToLoadingScene();
    }

    // Call this method when a chart is completed
    public void OnCurrentProgressEndSignalReceived()
    {
        CurrentPlayCount++;
        if (CurrentPlayCount >= TargetPlayCount)
        {
            // All charts completed, show result page
            
            // Clear current charts for next session
            CurrentCharts.Clear();
        }
        else
        {
            SceneManager.Instance.ChangeToLoadingScene();
        }
    }
}
