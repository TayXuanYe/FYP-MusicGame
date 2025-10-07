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
	private PackedScene _loadingScene;
	private PackedScene _resultScene;
	private PackedScene _setDefaultDifficultyScene;
	private PackedScene _collectDataPage;
	private PackedScene _reportBugScene;
	private PackedScene _settingScene;

	public override void _Ready()
	{
		_instance = this;

		_loginScene = GD.Load<PackedScene>("res://Scene/login_scene.tscn");
		_registerScene = GD.Load<PackedScene>("res://Scene/register_scene.tscn");
		_mainMenuScene = GD.Load<PackedScene>("res://Scene/main_scene.tscn");
		_gameScene = GD.Load<PackedScene>("res://Scene/game_scene.tscn");
		_loadingScene = GD.Load<PackedScene>("res://Scene/loading_scene.tscn");
		_resultScene = GD.Load<PackedScene>("res://Scene/result_scene.tscn");
		_setDefaultDifficultyScene = GD.Load<PackedScene>("res://Scene/set_default_difficulty_scene.tscn");
		_collectDataPage = GD.Load<PackedScene>("res://Scene/CollectDataPage.tscn");
		_reportBugScene = GD.Load<PackedScene>("res://Scene/report_bug_scene.tscn");
		_settingScene = GD.Load<PackedScene>("res://Scene/setting_scene.tscn");
	}

	public void ChangeToLoginScene()
	{
		if (_loginScene != null)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _loginScene);
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
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _registerScene);
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
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _mainMenuScene);
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
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _gameScene);
		}
		else
		{
			GD.PrintErr("GameScene is not loaded!");
		}
	}

	public void ChangeToLoadingScene()
	{
		if (_loadingScene != null)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _loadingScene);
		}
		else
		{
			GD.PrintErr("LoadingScene is not loaded!");
		}
	}

	public void ChangeToResultScene()
	{
		if (_resultScene != null)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _resultScene);
			CallDeferred(nameof(DelayedInitializeResultScene));
		}
		else
		{
			GD.PrintErr("ResultScene is not loaded!");
		}
	}

	public void ChangeToResultScene(int historyId)
	{
		if (_resultScene != null)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _resultScene);
			CallDeferred(nameof(DelayedInitializeResultScene), historyId);
		}
		else
		{
			GD.PrintErr("ResultScene is not loaded!");
		}
	}

	public void DelayedInitializeResultScene(int historyId)
	{
		var currentScene = GetTree().CurrentScene;
		if (currentScene is ResultScene resultScene)
		{
			resultScene.Initialize(historyId);
		}
		else
		{
			GD.PrintErr("Current scene is not ResultScene!");
		}
	}

	public void DelayedInitializeResultScene()
	{
		var currentScene = GetTree().CurrentScene;
		if (currentScene is ResultScene resultScene)
		{
			resultScene.Initialize();
		}
		else
		{
			GD.PrintErr("Current scene is not ResultScene!");
		}
	}

	public void ChangeToSetDefaultDifficultyScene()
	{
		if (_setDefaultDifficultyScene != null)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _setDefaultDifficultyScene);
		}
		else
		{
			GD.PrintErr("SetDefaultDifficultyScene is not loaded!");
		}
	}

	public void ChangeToReportBugScene()
	{
		if (_reportBugScene != null)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _reportBugScene);
		}
		else
		{
			GD.PrintErr("ReportBugScene is not loaded!");
		}
	}

	public void ChangeToSettingScene()
	{
		if (_settingScene != null)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _settingScene);
		}
		else
		{
			GD.PrintErr("SettingScene is not loaded!");
		}
	}

	public void ChangeToCollectDataPage()
	{
		GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _collectDataPage);
	}
}
