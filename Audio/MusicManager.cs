using UnityEngine;
using System.Collections;

public static class MusicManager
{
	private static MusicTrack _Music;
	private static MusicTrack Music
	{
		get
		{
			if (_Music == null)
			{
				GameObject musicObject = new GameObject("Music Track", typeof(AudioSource), typeof(MusicTrack));
				_Music = musicObject.GetComponent<MusicTrack>();
				_Music.dontDestroy();
				_Music.audio.mute = !GameSettings.MusicIsOn;
			}	
			return _Music;
		}
	}
	
	public static float VolumeMultiplier = 1.0f;
	
	public static void Play(AudioClip musicClip)
	{
		Music.play(musicClip);
		Music.targetVolume = 1f * VolumeMultiplier;
	}
	
	public static void Play()
	{
		Music.play();
	}
	
	public static void Pause()
	{
		Music.pause();
	}
	
	public static void Stop()
	{
		Music.stop();
	}
	
	public static AudioClip Clip
	{
		get { return Music.clip; }
	}
	
	public static bool IsPlaying
	{
		get { return Music.isPlaying; }
	}
	
	public static float Volume
	{
		get { return Music.volume; }
		set { Music.volume = value * VolumeMultiplier; }
	}
	
	public static void fadeOut(float fadeTime)
	{
		Music.fadeTo(0, fadeTime);
	}
	
	public static void fadeTo(float targetVolume, float fadeTime)
	{
		Music.fadeTo(targetVolume * VolumeMultiplier, fadeTime);
	}
	
}
