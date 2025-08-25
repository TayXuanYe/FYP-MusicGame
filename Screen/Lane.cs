using Godot;
using System;
using System.Collections.Generic;

public partial class Lane : Node2D
{
	[Export] public string KeyCode { get; set; }
	[Export] private Line2D _laneLine;
	[Export] private TextureRect _hittingArea;
	[Export] private Label _laneLabel;
	[Export] private PackedScene _tapNoteScene;
	[Export] private PackedScene _holdNoteScene;

	[Export] private float _laneWidth = 100f;
	[Export] private float _hittingAreaHeight = 80f;
	[Export] private float _laneLineWidth = 2f;
	[Export] private int _fontSize = 32;
	[Export] private Vector2 _startPoint = new Vector2(0, 0);
	[Export] private Texture2D _normalTexture;
	[Export] private Texture2D _pressedTexture;
	[Export] private Color _normalHintTextColor;
	[Export] private Color _pressedHintTextColor;

	public Queue<(double targetHittedTime, Color noteColor, string type, double durationTime)> NotesMetadataQueue { get; private set; }
	private Queue<TapNote> _tapNotesQueue = new Queue<TapNote>();
	private Queue<HoldNote> _holdNotesQueue = new Queue<HoldNote>();
	private int _tapNoteIdCounter = 0;
	private int _holdNoteIdCounter = 0;
	private double _currentTime = 0f;
	private float _spawnNoteYPosition = -100f;

	[Signal] public delegate void DisplayHitResultEventHandler(string resultText);

	public override void _Ready()
	{
		InitLane();

		// get the notes metadata queue
		// NotesMetadataQueue = ChartManager.Instance.GetNotesMetadata(laneIndex);

		//temp
		NotesMetadataQueue = new Queue<(double, Color, string, double)>();
		NotesMetadataQueue.Enqueue((5.0, Colors.Red, "Hold", 1));
		NotesMetadataQueue.Enqueue((10.0, Colors.Blue, "Tap", 0));
		NotesMetadataQueue.Enqueue((15.0, Colors.Green, "Tap", 0));
		NotesMetadataQueue.Enqueue((20.0, Colors.Yellow, "Tap", 0));
		NotesMetadataQueue.Enqueue((25.0, Colors.Purple, "Tap", 0));
		NotesMetadataQueue.Enqueue((30.0, Colors.Orange, "Tap", 0));
	}

	private void InitLane()
	{
		Vector2 viewportSize = GetViewportRect().Size;

		// Draw lane line
		_laneLine.Width = _laneLineWidth;
		Vector2 leftEndPoint = new Vector2(0, viewportSize.Y);
		Vector2 leftHittingBoxPoint = new Vector2(0, viewportSize.Y - _hittingAreaHeight);
		Vector2 rightHittingBoxPoint = new Vector2(_laneWidth, viewportSize.Y - _hittingAreaHeight);
		Vector2 rightStartPoint = new Vector2(_laneWidth, 0);
		Vector2 rightEndPoint = new Vector2(_laneWidth, viewportSize.Y);

		_laneLine.ClearPoints();
		_laneLine.AddPoint(_startPoint);
		_laneLine.AddPoint(leftEndPoint);
		_laneLine.AddPoint(leftHittingBoxPoint);
		_laneLine.AddPoint(rightHittingBoxPoint);
		_laneLine.AddPoint(rightStartPoint);
		_laneLine.AddPoint(rightEndPoint);

		// Setup hitting area
		_hittingArea.Size = new Vector2(_laneWidth, _hittingAreaHeight);
		_hittingArea.Position = new Vector2(0, viewportSize.Y - _hittingAreaHeight);
		_hittingArea.Texture = _normalTexture;

		// Setup lane label
		_laneLabel.Position = new Vector2(_laneWidth / 2, viewportSize.Y - _hittingAreaHeight / 2);
		_laneLabel.Text = KeyCode;
		_laneLabel.LabelSettings.FontSize = _fontSize;
		_laneLabel.LabelSettings.OutlineSize = 2;
	}

	public override void _Process(double delta)
	{
		_currentTime += delta;

		// spawn notes
		if (NotesMetadataQueue.Count > 0)
		{
			while (CalculateNoteSpawnTime(NotesMetadataQueue.Peek().targetHittedTime) <= _currentTime)
			{
				var noteMetadata = NotesMetadataQueue.Dequeue();
				if (noteMetadata.type == "Tap")
				{
					SpawnTapNote(noteMetadata.targetHittedTime, noteMetadata.noteColor);
				}
				else if (noteMetadata.type == "Hold")
				{
					SpawnHoldNote(noteMetadata.targetHittedTime, noteMetadata.durationTime, noteMetadata.noteColor);
				}

				if (NotesMetadataQueue.Count == 0)
				{
					break;
				}
			}
		}

		// check for missed tap notes
		if (_tapNotesQueue != null && _tapNotesQueue.Count > 0)
		{
			while (_tapNotesQueue.Peek().TargetHittedTime + GameSetting.Instance.GoodTimeRange < _currentTime)
			{
				var tapNote = _tapNotesQueue.Dequeue();

				// Handle missed note logic here
				EmitSignal(SignalName.DisplayHitResult, "Miss");
				DestroyedTapNote(tapNote);
				if (_tapNotesQueue.Count == 0)
				{
					break;
				}
			}
		}

		if (_holdNotesQueue != null && _holdNotesQueue.Count > 0)
		{
			// check for missed hold tap notes
			while (_holdNotesQueue.Peek().TargetHittedTime + GameSetting.Instance.GoodTimeRange < _currentTime && !_holdNotesQueue.Peek().IsTapNoteTriggered)
			{
				EmitSignal(SignalName.DisplayHitResult, "Miss");
				if (_holdNotesQueue.Count == 0)
				{
					break;
				}
			}
		}
	}

	private double CalculateNoteSpawnTime(double targetHittedTime)
	{
		// Calculate the time when the note should spawn based on the target hit time and the note speed
		double laneHeight = GetViewportRect().Size.Y;
		double distanceToTravel = laneHeight - _hittingAreaHeight - _spawnNoteYPosition;
		double spawnTime = targetHittedTime - (distanceToTravel / GameSetting.Instance.NoteSpeed);
		return spawnTime;
	}

	public void OnKeyPressed()
	{
		_hittingArea.Texture = _pressedTexture;
		_laneLabel.Modulate = _pressedHintTextColor;
		CheckNoteHit();
	}

	public void OnKeyReleased()
	{
		_hittingArea.Texture = _normalTexture;
		_laneLabel.Modulate = _normalHintTextColor;
	}

	private void CheckNoteHit()
	{
		// tap note
		if (_tapNotesQueue != null && _tapNotesQueue.Count > 0)
		{
			var tapNote = _tapNotesQueue.Peek();
			var hitResult = tapNote.CheckNoteHit();

			if (hitResult.isTrigger)
			{
				// Handle the hit result
				EmitSignal(SignalName.DisplayHitResult, hitResult.hitResult);
				AudioManager.Instance.PlaySound(GameSetting.Instance.TapSoundEffect);

				_tapNotesQueue.Dequeue();
				DestroyedTapNote(tapNote);
			}
		}

		// hold note
		if (_holdNotesQueue != null && _holdNotesQueue.Count > 0)
		{
			var holdNote = _holdNotesQueue.Peek();
			if (!holdNote.IsTapNoteTriggered)
			{
				var hitResult = holdNote.CheckNoteHit();

				if (hitResult.isTrigger)
				{
					// Handle the hit result
					EmitSignal(SignalName.DisplayHitResult, hitResult.hitResult);
					AudioManager.Instance.PlaySound(GameSetting.Instance.TapSoundEffect);
				}
			}
			else
			{
				// maybe continue hold ...
			}
		}
	}

	private void CheckNoteRelease()
	{

	}

	private void SpawnTapNote(double targetHittedTime, Color noteColor)
	{
		TapNote tapNote = _tapNoteScene.Instantiate<TapNote>();
		string tapNoteId = KeyCode + "_Tap_" + _tapNoteIdCounter;
		tapNote.Initialize(_currentTime, noteColor, targetHittedTime, tapNoteId);
		_tapNoteIdCounter++;
		AddChild(tapNote);
		_tapNotesQueue.Enqueue(tapNote);
		tapNote.Position = new Vector2(_laneWidth / 2, _spawnNoteYPosition); // Set the initial position at the top of the lane
	}

	private void DestroyedTapNote(TapNote tapNote)
	{
		tapNote.Destroyed();
	}

	private void SpawnHoldNote(double targetHittedTime, double durationTime, Color noteColor)
	{
		HoldNote holdNote = _holdNoteScene.Instantiate<HoldNote>();
		string holdNoteId = KeyCode + "_Hold_" + _holdNoteIdCounter;
		string holdTapNoteId = KeyCode + "_Tap_" + _tapNoteIdCounter;
		holdNote.Initialize(_currentTime, noteColor, targetHittedTime, durationTime, holdNoteId, holdTapNoteId);
		_tapNoteIdCounter++;
		_holdNoteIdCounter++;
		AddChild(holdNote);
		_holdNotesQueue.Enqueue(holdNote);
		holdNote.Position = new Vector2(_laneWidth / 2, _spawnNoteYPosition - holdNote.LengthOfShadow);
	}
	
	private void DestroyedHoldNote(HoldNote holdNote)
	{
		holdNote.Destroyed();
	}
}
