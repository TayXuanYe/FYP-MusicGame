using Godot;
using System;

public partial class TapNote : Area2D
{
	[Export] private Color _noteColor;
	[Export] private Panel _notePanel;
	[Export] public double TargetHitTime { get; private set; }
	public string Id { get; private set; }
	private double _currentTime;
	private bool _isDestroyed = false;

	public void Initialize(double currentTime, Color noteColor, double targetHittedTime, string id)
	{
		_currentTime = currentTime;
		_noteColor = noteColor;
		TargetHitTime = targetHittedTime;
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

	public (bool isTrigger, string hitResult, double hitTime, double timeDifference, double targetHittedTime) CheckNoteHit()
	{
		// time difference be nagative if is too fast else positive
		double timeDifference = _currentTime - TargetHitTime;

		if (Math.Abs(timeDifference) <= GameSetting.Instance.CriticalPerfectTimeRange)
		{
			return (true, "Critical Perfect", _currentTime, timeDifference, TargetHitTime);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.PerfectTimeRange)
		{
			return (true, "Perfect", _currentTime, timeDifference, TargetHitTime);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.GreatTimeRange)
		{
			return (true, "Great", _currentTime, timeDifference, TargetHitTime);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.GoodTimeRange)
		{
			return (true, "Good", _currentTime, timeDifference, TargetHitTime);
		}

		return (false, null, 0, 0, 0);
	}

	public void Destroyed()
	{
		_isDestroyed = true;
		Modulate = Colors.Transparent;
		QueueFree();
	}

	public Vector2 GetNoteSize()
	{
		return _notePanel.Size;
	}
}
