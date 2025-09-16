using Godot;
using System;

public partial class SceneManager : Node
{
	private static SceneManager _instance;
	public static SceneManager Instance => _instance;

	private PackedScene _loginScene;
	private PackedScene _registerScene;
	private PackedScene _mainMenuScene;
	private PackedScene _gameScene;

	public override void _Ready()
	{
		_instance = this;

		_loginScene = GD.Load<PackedScene>("res://Scene/login_scene.tscn");
		_registerScene = GD.Load<PackedScene>("res://Scene/register_scene.tscn");
		_mainMenuScene = GD.Load<PackedScene>("res://Scene/main_scene.tscn");
		_gameScene = GD.Load<PackedScene>("res://Scene/game_scene.tscn");
	}

	public void ChangeToLoginScene()
	{
		if (_loginScene != null)
		{
			GetTree().ChangeSceneToPacked(_loginScene);
		}
		else
		{
			GD.PrintErr("LoginScene is not loaded!");
		}
	}

	public void ChangeToRegisterScene()
	{
		if (_registerScene != null)
		{
			GetTree().ChangeSceneToPacked(_registerScene);
		}
		else
		{
			GD.PrintErr("RegisterScene is not loaded!");
		}
	}

	public void ChangeToMainMenuScene()
	{
		if (_mainMenuScene != null)
		{
			GetTree().ChangeSceneToPacked(_mainMenuScene);
		}
		else
		{
			GD.PrintErr("MainMenuScene is not loaded!");
		}
	}
	
	public void ChangeToGameScene()
	{
		if (_gameScene != null)
		{
			GetTree().ChangeSceneToPacked(_gameScene);
		}
		else
		{
			GD.PrintErr("GameScene is not loaded!");
		}
	}
}
