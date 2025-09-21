using Godot;
using System;

public partial class HoldNote : Area2D
{
	[Export] private Panel _HoldShadow;

	[Export] private Color _noteColor;
	[Export] public double TargetHitTime { get; private set; }
	[Export] public double HoldDuration { get; private set; }
	public string HoldNoteId { get; private set; }
	private double _currentTime;
	private bool _isDestroyed = false;
	private bool _isHolding = false;
	private double _holdTime;
	public float LengthOfShadow { get; private set; }

	public void Initialize(double currentTime, Color noteColor, double targetHittedTime, double holdDuration, string holdNoteId)
	{
		_currentTime = currentTime;
		_noteColor = noteColor;
		TargetHitTime = targetHittedTime;
		HoldDuration = holdDuration;
		HoldNoteId = holdNoteId;
	}

	public override void _Ready()
	{
		// Set the color of the note
		_HoldShadow.Modulate = _noteColor with { A = 0.7f };

		// calculate the length of the shadow based on the hold duration and note speed
		LengthOfShadow = (float)(GameSetting.Instance.NoteSpeed * HoldDuration);
		_HoldShadow.Size = new Vector2(_HoldShadow.Size.X, LengthOfShadow);
	}

	public override void _Process(double delta)
	{
		if (_isDestroyed) { return; }
		_currentTime += delta;
		MoveNote();
		if (_isHolding)
		{
			_holdTime += delta;
			// change the color to bright when holding
			_HoldShadow.Modulate = _noteColor with { A = 0.9f };
		}
		else if (_currentTime > TargetHitTime)
		{
			// change the color more dark when not holding and missing
			_HoldShadow.Modulate = _noteColor with { A = 0.7f };
		}

	}

	private void MoveNote()
	{
		// Move the note downwards at the specified speed
		Position += new Vector2(0, (float)(GameSetting.Instance.NoteSpeed * GetProcessDeltaTime()));
	}

	public void OnNotePressed()
	{
		_isHolding = true;
	}

	public void OnNoteReleased()
	{
		_isHolding = false;
	}

	public void Destroyed()
	{
		_isDestroyed = true;
		QueueFree();
	}
	
	public (string hitResult, double holdTime, double holdRatio) GetHoldResult()
	{
		double holdRatio = _holdTime / HoldDuration;
		if (holdRatio >= GameSetting.Instance.CriticalPerfectHoldDurationRatio)
		{
			return ("Critical Perfect", _holdTime, holdRatio);
		}
		else if (holdRatio >= GameSetting.Instance.PerfectHoldDurationRatio)
		{
			return ("Perfect", _holdTime, holdRatio);
		}
		else if (holdRatio >= GameSetting.Instance.GreatHoldDurationRatio)
		{
			return ("Great", _holdTime, holdRatio);
		}
		else if (holdRatio >= GameSetting.Instance.GoodHoldDurationRatio)
		{
			return ("Good", _holdTime, holdRatio);
		}
		else
		{
			return ("Miss", _holdTime, holdRatio);
		}
	}
}
