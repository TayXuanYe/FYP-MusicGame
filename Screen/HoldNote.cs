using Godot;
using System;

public partial class HoldNote : Area2D
{
	[Export] private Panel _TapNote;
	[Export] private Panel _HoldShadow;

	[Export] private Color _noteColor;
	[Export] public double TargetHittedTime { get; private set; }
	[Export] private double _holdDuration;
	public string Id { get; private set; }
	private double _currentTime;
	private bool _isDestroyed = false;
	private bool _isHolding = false;
	private double _holdTime;
	public float LengthOfShadow { get; private set; }

	public void Initialize(double currentTime, Color noteColor, double targetHittedTime, double holdDuration, string id)
	{
		_currentTime = currentTime;
		_noteColor = noteColor;
		TargetHittedTime = targetHittedTime;
		_holdDuration = holdDuration;
		Id = id;
	}

	public override void _Ready()
	{
		// Set the color of the note
		_TapNote.Modulate = _noteColor;
		_HoldShadow.Modulate = _noteColor with { A = 0.5f };

		// calculate the length of the shadow based on the hold duration and note speed
		LengthOfShadow = (float)(GameSetting.Instance.NoteSpeed * _holdDuration);
		_HoldShadow.Size = new Vector2(_TapNote.Size.X, LengthOfShadow);
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

		// Tap note position fixed at the jugment line when it reaches the target hitted time and during the hold duration
		if (_currentTime >= TargetHittedTime && _currentTime <= TargetHittedTime + _holdDuration)
		{
			float screenHeight = GetViewportRect().Size.Y;
			_TapNote.GlobalPosition = new Vector2(Position.X, screenHeight - 80 - _TapNote.Size.Y / 2);
		}
	}

	public (bool isTrigger, string hitResult, double hitTime, double timeDifference) CheckNoteHit()
	{
		// timedifference be nagative if is too fast else positive
		double timeDifference = _currentTime - TargetHittedTime;

		if (Math.Abs(timeDifference) <= GameSetting.Instance.CriticalPerfectTimeRange)
		{
			return (true, "Critical Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.PerfectTimeRange)
		{
			return (true, "Perfect", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.GreatTimeRange)
		{
			return (true, "Great", _currentTime, timeDifference);
		}
		else if (Math.Abs(timeDifference) <= GameSetting.Instance.GoodTimeRange)
		{
			return (true, "Good", _currentTime, timeDifference);
		}

		return (false, null, 0, 0);
	}

	public void CheckNoteRelease()
	{

	}
}
