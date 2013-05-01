using UnityEngine;
using System.Collections;

public class GameSettings
{
	private static string SOUND_KEY = "SoundOn";
	private static string MUSIC_KEY = "MusicOn";
	
	static GameSettings()
	{
		SoundOn = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
		MusicOn = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;
	}

	private static bool SoundOn = true;
	public static bool SoundIsOn
	{
		get { return SoundOn; }
		set
		{
			SoundOn = value;
			PlayerPrefs.SetInt(SOUND_KEY, SoundOn ? 1 : 0);
		}
	}
	public static void ToggleSound()
	{
		SoundOn = !SoundOn;
		PlayerPrefs.SetInt(SOUND_KEY, SoundOn ? 1 : 0);
	}
	
	private static bool MusicOn = true;
	public static bool MusicIsOn
	{
		get { return MusicOn; }
		set
		{
			MusicOn = value;
			PlayerPrefs.SetInt(MUSIC_KEY, MusicOn ? 1 : 0);
		}
	}
	public static void ToggleMusic()
	{
		MusicOn = !MusicOn;
		PlayerPrefs.SetInt(MUSIC_KEY, MusicOn ? 1 : 0);
	}
	
	public static void LoadSettings()
	{
		SoundOn = (PlayerPrefs.GetInt(SOUND_KEY, 1) == 1);
		MusicOn = (PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1);
	}
	
}