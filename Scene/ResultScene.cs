using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class ResultScene : Control
{
    [Export] private Label _titleLabel;
    [Export] private Label _artistLabel;
    [Export] private Label _difficultyLabel;
    [Export] private Label _levelLabel;
    [Export] private Label _scoreLabel;
    [Export] private Label _maxComboLabel;
    [Export] private Label _accuracyLabel;
    [Export] private Label _finalAttentionLabel;
    [Export] private Label _tapCriticalPerfectLabel;
    [Export] private Label _tapPerfectLabel;
    [Export] private Label _tapGreatLabel;
    [Export] private Label _tapGoodLabel;
    [Export] private Label _tapMissLabel;
    [Export] private Label _holdCriticalPerfectLabel;
    [Export] private Label _holdPerfectLabel;
    [Export] private Label _holdGreatLabel;
    [Export] private Label _holdGoodLabel;
    [Export] private Label _holdMissLabel;

    [Export] private Button _backButton;
    [Export] private Button _nextButton;
    [Export] private Button _exitButton;
    private int _currentIndex = 0;
    public override void _Ready()
    {
        _backButton.Pressed += OnBackButtonPressed;
        _nextButton.Pressed += OnNextButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;
        DisplayResult(_currentIndex);
        
    }

    private void DisplayResult(int index)
    {
        // var resultData = gameData.ResultData;
        var songData = ChartManager.Instance.LoadChart(GameProgressManger.Instance.PlaylistChartsId[index]);
        var resultData = GameProgressManger.Instance.RawUserInputData[index];

        int tapCriticalPerfectCount = 0;
        int tapPerfectCount = 0;
        int tapGreatCount = 0;
        int tapGoodCount = 0;
        int tapMissCount = 0;

        int holdCriticalPerfectCount = 0;
        int holdPerfectCount = 0;
        int holdGreatCount = 0;
        int holdGoodCount = 0;
        int holdMissCount = 0;

        int maxCombo = 0;
        double totalSimulateScore = 0;
        int totalSimulateCount = 0;
        List<double> timeDifferences = new List<double>();
        foreach (ProcessResult processResult in resultData)
        {
            int currentCombo = 0;
            if (processResult.NoteType == "Tap")
            {
                totalSimulateCount++;
                timeDifferences.Add(processResult.TimeDifference);
                switch (processResult.HitResult)
                {
                    case "CriticalPerfect":
                        totalSimulateScore += 1;
                        tapCriticalPerfectCount++;
                        currentCombo++;
                        break;
                    case "Perfect":
                        totalSimulateScore += 0.9;
                        tapPerfectCount++;
                        currentCombo++;
                        break;
                    case "Great":
                        totalSimulateScore += 0.7;
                        tapGreatCount++;
                        currentCombo++;
                        break;
                    case "Good":
                        totalSimulateScore += 0.5;
                        tapGoodCount++;
                        currentCombo++;
                        break;
                    case "Miss":
                        if (currentCombo > maxCombo)
                        {
                            maxCombo = currentCombo;
                        }
                        currentCombo = 0;
                        tapMissCount++;
                        break;
                }
            }
            else if (processResult.NoteType == "Hold")
            {
                double duration = processResult.DurationTime;
                int count = 1;
                while (duration > 0)
                {
                    duration -= count * 0.3;
                    count++;
                }
                totalSimulateCount+= count;
                switch (processResult.HitResult)
                {
                    case "CriticalPerfect":
                        totalSimulateScore += count * 1;
                        holdCriticalPerfectCount++;
                        currentCombo++;
                        break;
                    case "Perfect":
                        totalSimulateScore += count * 0.9;
                        holdPerfectCount++;
                        currentCombo++;
                        break;
                    case "Great":
                        totalSimulateScore += count * 0.7;
                        holdGreatCount++;
                        currentCombo++;
                        break;
                    case "Good":
                        totalSimulateScore += count * 0.5;
                        holdGoodCount++;
                        currentCombo++;
                        break;
                    case "Miss":
                        holdMissCount++;
                        if (currentCombo > maxCombo)
                        {
                            maxCombo = currentCombo;
                        }
                        currentCombo = 0;
                        break;
                }
            }
        }
        if(timeDifferences.Count == 0)
        {
            timeDifferences.Add(0);
        }
        double average = timeDifferences.Average();
        double variance = timeDifferences.Sum(d => Math.Pow(d - average, 2)) / timeDifferences.Count();
        double std = Math.Sqrt(variance);

        _titleLabel.Text = songData.Title;
        _artistLabel.Text = songData.Artist;
        _difficultyLabel.Text = songData.Difficulty;
        _levelLabel.Text = songData.Level.ToString("F2");

        _scoreLabel.Text = (totalSimulateCount/totalSimulateScore*1000000).ToString("F0");
        _maxComboLabel.Text = maxCombo.ToString();
        _accuracyLabel.Text = $"{std*1000} ms";
        // _finalAttentionLabel.Text = resultData.FinalAttention.ToString("F2");

        _tapCriticalPerfectLabel.Text = tapCriticalPerfectCount.ToString();
        _tapPerfectLabel.Text = tapPerfectCount.ToString();
        _tapGreatLabel.Text = tapGreatCount.ToString();
        _tapGoodLabel.Text = tapGoodCount.ToString();
        _tapMissLabel.Text = tapMissCount.ToString();
        
        _holdCriticalPerfectLabel.Text = holdCriticalPerfectCount.ToString();
        _holdPerfectLabel.Text = holdPerfectCount.ToString();
        _holdGreatLabel.Text = holdGreatCount.ToString();
        _holdGoodLabel.Text = holdGoodCount.ToString();
        _holdMissLabel.Text = holdMissCount.ToString();
    }

    private void OnBackButtonPressed()
    {
        if(_currentIndex > 0)
        {
            _currentIndex--;
            DisplayResult(_currentIndex);
        }
    }

    private void OnNextButtonPressed()
    {
        if(_currentIndex < GameProgressManger.Instance.PlaylistChartsId.Count - 1)
        {
            _currentIndex++;
            DisplayResult(_currentIndex);
        }
    }

    private void OnExitButtonPressed()
    {
        SceneManager.Instance.ChangeToMainMenuScene();
    }
}
