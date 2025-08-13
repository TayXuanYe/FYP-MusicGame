using Godot;
using System;

public partial class SceneManager : Node
{
	private PackedScene _loginScreen;
	private PackedScene _registerScreen;

	public override void _Ready()
	{
		_loginScreen = GD.Load<PackedScene>("res://Screen/LoginScreen.tscn");
		_registerScreen = GD.Load<PackedScene>("res://Screen/RegisterScreen.tscn");
	}

	public void ChangeToLoginScreen()
	{
		if (_loginScreen != null)
		{
			GetTree().ChangeSceneToPacked(_loginScreen);
		}
		else
		{
			GD.PrintErr("LoginScreen is not loaded!");
		}
	}

	public void ChangeToRegisterScreen()
	{
		if (_registerScreen != null)
		{
			GetTree().ChangeSceneToPacked(_registerScreen);
		}
		else
		{
			GD.PrintErr("RegisterScreen is not loaded!");
		}
	}
}
