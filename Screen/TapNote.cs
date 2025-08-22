using Godot;
using System;

public partial class TapNote : Area2D
{
	// unit px per second
	[Export] private double _noteSpeed;
	[Export] private Color _noteColor;
	[Export] private double _goodTimeRange = 0.15f; // seconds
	[Export] private double _greatTimeRange = 0.1f;
	[Export] private double _perfectTimeRange = 0.05f;
	[Export] private double _criticalPerfectTimeRange = 0.02f;
	[Export] public double TargetHittedTime { get; private set; }
	public string Id { get; private set; }
	private double _currentTime;
	private bool _isDestroyed = false;

	public void Initialize(float noteSpeed, double currentTime, Color noteColor, double targetHittedTime, string id)
	{
		_noteSpeed = noteSpeed;
		_currentTime = currentTime;
		_noteColor = noteColor;
		TargetHittedTime = targetHittedTime;
		Id = id;
	}

	public override void _Ready()
	{
		// Set the color of the note
		Modulate = _noteColor;
	}

	public override void _Process(double delta)
	{
		_currentTime += delta;
		if (!_isDestroyed)
		{
			MoveNote();
		}
	}

	private void MoveNote()
	{
		// Move the note downwards at the specified speed
		Position += new Vector2(0, (float)(_noteSpeed * GetProcessDeltaTime()));
	}

	public (bool isTrigger, string hitResult, double hitTime, double timeDifference) CheckNoteHit()
	{
		// timedifference be nagative if is too fast else positive
		double timeDifference = _currentTime - TargetHittedTime;

		if (Math.Abs(timeDifference) <= _criticalPerfectTimeRange)
		{
			PlayHitEffect("Critical Perfect");
			return (true, "Critical Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= _perfectTimeRange)
		{
			PlayHitEffect("Perfect");
			return (true, "Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= _greatTimeRange)
		{
			PlayHitEffect("Great");
			return (true, "Great", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= _goodTimeRange)
		{
			PlayHitEffect("Good");
			return (true, "Good", _currentTime, timeDifference);
		}

		return (false, null, 0, 0);
	}

	public void Destroyed()
	{
		_isDestroyed = true;
		Modulate = Colors.Transparent;
		QueueFree();
	}

	private void PlayHitEffect(string displayText)
	{

	}
}
