using Godot;
using System;

public partial class TapNote : Area2D
{
    // unit px per second
    [Export] private float _noteSpeed = 200f;
    // unit px
    [Export] private float _judgmentLineVerticalDistance = 1000f;
    [Export] private Color _noteColor;
    [Export] private float _goodTimeRange = 0.15f; // seconds
    [Export] private float _greatTimeRange = 0.1f;
    [Export] private float _perfectTimeRange = 0.05f;
    [Export] private float _criticalPerfectTimeRange = 0.02f;
    [Export] private double _targetHittedTime;
    private double _currentTime;

    public TapNote(double currentTime, Color noteColor, double targetHittedTime)
    {
        _currentTime = currentTime;
        _noteColor = noteColor;
        _targetHittedTime = targetHittedTime;
    }

    public override void _Ready()
    {
        // Set the color of the note
        Modulate = _noteColor;

        // Set the position of the note at the top of the screen
        Position = new Vector2(0, -30);
    }

    public override void _Process(double delta)
    {
        _currentTime += delta;
        MoveNote();
    }

    private void MoveNote()
    {
        // Move the note downwards at the specified speed
        Position += new Vector2(0, _noteSpeed * (float)GetProcessDeltaTime());
    }

    public int CheckNoteHit()
    {
        double timeDifference = _currentTime - _targetHittedTime;

        if (Math.Abs(timeDifference) <= _criticalPerfectTimeRange)
        {
            QueueFree();
            return 4; // Critical Perfect
        }
        else if (Math.Abs(timeDifference) <= _perfectTimeRange)
        {
            QueueFree();
            return 3; // Perfect
        }
        else if (Math.Abs(timeDifference) <= _greatTimeRange)
        {
            QueueFree();
            return 2; // Great
        }
        else if (Math.Abs(timeDifference) <= _goodTimeRange)
        {
            QueueFree();
            return 1; // Good
        }

        return 0;
    }
}
