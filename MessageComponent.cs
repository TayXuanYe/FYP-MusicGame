using Godot;
using System;

public partial class MessageComponent : Control
{
	[Export] public Label MessageLabel { get; set; }
	[Export] public Button ClickButton { get; set; }
}
