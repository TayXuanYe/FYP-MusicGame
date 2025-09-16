using Godot;
using System;
using System.Collections.Generic;

public partial class GameProgressManger : Node
{
    public static GameProgressManger Instance { get; private set; }
    public List<ChartData> CurrentCharts { get; set; } = new();
    private int _targetPlayCount = 4;
    public int CurrentPlayCount = 0;
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
    }

    // Signal progress started
    private void OnProgressStartedSignalReceived()
    {
        // init current play count
        CurrentPlayCount = 0;

        if (level != 0f)
        {
            List<int> chartIds = ChartManager.Instance.GetChartIdsByLevel(level, _targetPlayCount);
            foreach (var chartId in chartIds)
            {
                ChartData chartData = ChartManager.Instance.LoadChart(chartId);
                if (chartData != null)
                {
                    CurrentCharts.Add(chartData);
                }
            }
            _targetPlayCount = CurrentCharts.Count;
        }

        SceneManager.Instance.ChangeToGameScene();
        // emit ready signal
        SignalManager.Instance.EmitChartDataLoadedReady();
    }

    // Call this method when a chart is completed
    public void OnCurrentProgressEndSignalReceived()
    {
        CurrentPlayCount++;
        if (CurrentPlayCount >= _targetPlayCount)
        {
            // All charts completed, show result page
            
            // Clear current charts for next session
            CurrentCharts.Clear();
        }
        else
        {
            SceneManager.Instance.ChangeToGameScene();
        }
    }
}
