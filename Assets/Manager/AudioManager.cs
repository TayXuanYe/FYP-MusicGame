using Godot;
using System;

public partial class AudioManager : Node
{
	private static AudioManager _instance;
	public static AudioManager Instance => _instance;

	private const int AudioPoolSize = 15;
	private AudioStreamPlayer[] audioPlayers = new AudioStreamPlayer[AudioPoolSize];

	public override void _Ready()
	{
		_instance = this;

		for (int i = 0; i < AudioPoolSize; i++)
		{
			audioPlayers[i] = new AudioStreamPlayer();
			AddChild(audioPlayers[i]);
		}
	}

	public void PlaySound(AudioStream sound)
	{
		if (sound == null) return;

		foreach (var player in audioPlayers)
		{
			if (!player.Playing)
			{
				player.Stream = sound;
				player.Play();
				return;
			}
		}
	}
}
