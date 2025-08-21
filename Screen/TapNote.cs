using Godot;
using System;

public partial class TapNote : Area2D
{
	// unit px per second
	[Export] private float _noteSpeed = 200f;
	[Export] private Color _noteColor;
	[Export] private float _goodTimeRange = 0.15f; // seconds
	[Export] private float _greatTimeRange = 0.1f;
	[Export] private float _perfectTimeRange = 0.05f;
	[Export] private float _criticalPerfectTimeRange = 0.02f;
	[Export] public double TargetHittedTime { get; private set; }
	private double _currentTime;

	public void Initialize(float noteSpeed, double currentTime, Color noteColor, double targetHittedTime)
	{
		_noteSpeed = noteSpeed;
		_currentTime = currentTime;
		_noteColor = noteColor;
		TargetHittedTime = targetHittedTime;
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

	public (bool isTrigger,string hitResult, double hitTime, double timeDifference) CheckNoteHit()
	{
		double timeDifference = _currentTime - TargetHittedTime;

		if (Math.Abs(timeDifference) <= _criticalPerfectTimeRange)
		{
			return (true, "Critical Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= _perfectTimeRange)
		{
			return (true, "Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= _greatTimeRange)
		{
			return (true, "Great", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= _goodTimeRange)
		{
			return (true, "Good", _currentTime, timeDifference);
		}

		return (false, null, 0, 0);
	}
}
