using Godot;
using System;

public partial class TapNote : Area2D
{
	// unit px per second
	[Export] private Color _noteColor;
	[Export] public double TargetHittedTime { get; private set; }
	public string Id { get; private set; }
	private double _currentTime;
	private bool _isDestroyed = false;

	public void Initialize(double currentTime, Color noteColor, double targetHittedTime, string id)
	{
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
		Position += new Vector2(0, (float)(GameSetting.Instance.NoteSpeed * GetProcessDeltaTime()));
	}

	public (bool isTrigger, string hitResult, double hitTime, double timeDifference) CheckNoteHit()
	{
		// timedifference be nagative if is too fast else positive
		double timeDifference = _currentTime - TargetHittedTime;

		if (Math.Abs(timeDifference) <= GameSetting.Instance.CriticalPerfectTimeRange)
		{
			PlayHitEffect("Critical Perfect");
			return (true, "Critical Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.PerfectTimeRange)
		{
			PlayHitEffect("Perfect");
			return (true, "Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.GreatTimeRange)
		{
			PlayHitEffect("Great");
			return (true, "Great", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.GoodTimeRange)
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
