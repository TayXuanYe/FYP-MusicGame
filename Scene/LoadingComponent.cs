using Godot;
using System;

public partial class LoadingComponent : Control
{
	[Export] private TextureRect _loadingIcon;
	public override void _Ready()
	{
		Vector2 size = _loadingIcon.Size;
		
		_loadingIcon.PivotOffset = size / 2.0f;
	}
	public override void _Process(double delta)
	{
		_loadingIcon.Rotation += (float)(delta * 3); // Rotate at 3 radians per second
	}

}
