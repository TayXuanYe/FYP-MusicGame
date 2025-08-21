using Godot;
using System;
using System.Collections.Generic;

public partial class Lane : Node2D
{
	[Export] public string KeyCode { get; set; }
	[Export] private Line2D _laneLine;
	[Export] private TextureRect _hittingArea;
	[Export] private Label _laneLabel;

	[Export] private float _laneWidth = 100f;
	[Export] private float _hittingAreaHeight = 80f;
	[Export] private float _laneLineWidth = 2f;
	[Export] private int _fontSize = 32;
	[Export] private Vector2 _startPoint = new Vector2(0, 0);
	[Export] private Texture2D _normalTexture;
	[Export] private Texture2D _pressedTexture;
	[Export] private Color _normalHintTextColor;
	[Export] private Color _pressedHintTextColor;

	public float NoteSpeed { get; set; } = 200f;
	public Queue<(double targetHittedTime, Color noteColor, string type, double durationTime)> NotesMetadataQueue { get; private set; }
	private Queue<TapNote> _tapNotesQueue = new Queue<TapNote>();
	private double _currentTime = 0;

	public override void _Ready()
	{
		InitLane();

		// get the notes metadata queue
		// NotesMetadataQueue = ChartManager.Instance.GetNotesMetadata(laneIndex);
	}

	private void InitLane()
	{
		Vector2 viewportSize = GetViewportRect().Size;
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

		_hittingArea.Size = new Vector2(_laneWidth, _hittingAreaHeight);
		_hittingArea.Position = new Vector2(0, viewportSize.Y - _hittingAreaHeight);
		_hittingArea.Texture = _normalTexture;

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
			while (_tapNotesQueue.Peek().TargetHittedTime + 0.15f < _currentTime)
			{
				var tapNote = _tapNotesQueue.Dequeue();
				// Handle missed note logic here, e.g., play a sound or update score
				tapNote.QueueFree(); // Remove the note from the scene

				if (_tapNotesQueue.Count == 0)
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
		double spawnTime = targetHittedTime - ((laneHeight+100)/ NoteSpeed);
		return spawnTime;
	}

	public void OnKeyPressed()
	{
		_hittingArea.Texture = _pressedTexture;
		_laneLabel.Modulate = _pressedHintTextColor;
	}

	public void OnKeyReleased()
	{
		_hittingArea.Texture = _normalTexture;
		_laneLabel.Modulate = _normalHintTextColor;
	}

	public void CheckNoteHit()
	{
		if (_tapNotesQueue != null && _tapNotesQueue.Count > 0)
		{
			var tapNote = _tapNotesQueue.Peek();
			var hitResult = tapNote.CheckNoteHit();

			if (hitResult.isTrigger)
			{
				// Handle the hit result, e.g., update score, play sound, etc.
				GD.Print($"Hit Result: {hitResult.hitResult}, Hit Time: {hitResult.hitTime}, Time Difference: {hitResult.timeDifference}");
				tapNote.QueueFree(); // Remove the note from the scene
				_tapNotesQueue.Dequeue();
			}
		}
	}

	public void CheckNoteRelease()
	{

	}

	public void SpawnTapNote(double targetHittedTime, Color noteColor)
	{
		TapNote tapNote = new TapNote(NoteSpeed, _currentTime, noteColor, targetHittedTime);

		AddChild(tapNote);
		_tapNotesQueue.Enqueue(tapNote);
	}

	public void SpawnHoldNote(double targetHittedTime, double durationTime, Color noteColor)
	{
		// Logic to spawn a hold note
	}
}
