using Godot;
using System;

public partial class PauseSceneCustomeButton : Button
{
	[Export] private Label _buttonLabel;
	[Export] private Color _normalColor = new Color("#f8e5e5");
	[Export] private Color _hoverColor = new Color("#624b4b");
	[Export] private Color _pressedColor = new Color("#624b4b");

	public override void _Ready()
	{
		// Set initial label text color
		_buttonLabel.Modulate = _normalColor;

		// Connect signals for mouse events
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		Pressed += OnButtonPressed;
		ButtonUp += OnButtonReleased;
	}

	private void OnMouseEntered()
	{
		_buttonLabel.Modulate = _hoverColor;
	}

	private void OnMouseExited()
	{
		_buttonLabel.Modulate = _normalColor;
	}

	private void OnButtonPressed()
	{
		_buttonLabel.Modulate = _pressedColor;
	}
	
	private void OnButtonReleased()
	{
		// Change back to hover color if still hovering, else normal color
		if (GetGlobalMousePosition().DistanceTo(GetGlobalPosition()) < GetSize().Length())
		{
			_buttonLabel.Modulate = _hoverColor;
		}
		else
		{
			_buttonLabel.Modulate = _normalColor;
		}
	}
}
