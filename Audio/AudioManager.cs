using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	private static AudioManager main;
	public static AudioManager Main { get { return main; } }
	
	void Awake ()
	{
		if (main == null)
			main = this;
		
		UIOnly = false;
	}
	
	void OnDestroy ()
	{
		if (main == this)
			main = null;
	}
	
	
	public AudioClip music;
	private List<AudioSource> availableAudioSources, unavailableAudioSources, newlyAvailableAudioSources;
	public int audioSourceListSize = 7;
	
	public static bool UIOnly;
	
	void Start ()
	{
		if (MusicManager.Clip != music || !MusicManager.IsPlaying)
		{
			MusicManager.Play(music);
		}
		
		availableAudioSources = new List<AudioSource>( audioSourceListSize );
		for (int i = 0; i < audioSourceListSize; i++)
		{
			GameObject sfxObject = new GameObject("SFX " + (i+1).ToString(), typeof(AudioSource));
			sfxObject.audio.playOnAwake = false;
			sfxObject.transform.parent = this.transform;
			availableAudioSources.Add(sfxObject.audio);
		}
		unavailableAudioSources = new List<AudioSource>( audioSourceListSize );
		newlyAvailableAudioSources = new List<AudioSource>( audioSourceListSize );
	}
	
	void Update ()
	{
		foreach (AudioSource a in availableAudioSources)
		{
			a.mute = !GameSettings.SoundIsOn;
		}
		
		foreach (AudioSource a in unavailableAudioSources)
		{
			a.mute = !GameSettings.SoundIsOn;
			
			if (a.isPlaying == false)
			{
				newlyAvailableAudioSources.Add(a);
			}
		}
		
		foreach (AudioSource a in newlyAvailableAudioSources)
		{
			availableAudioSources.Add(a);
			unavailableAudioSources.Remove(a);
			a.loop = false;
		}
		
		newlyAvailableAudioSources.Clear();
	}	
	
	public static void PlaySFX(AudioClip audioClip, bool reuseAudioSourceIfAlreadyPlaying = true, bool isUISound = false, bool loop = false)
	{
		if (AudioManager.Main != null && audioClip != null)
		{
			AudioManager.Main.playSFX(audioClip, reuseAudioSourceIfAlreadyPlaying, isUISound, loop);
		}
	}
	
	public void playSFX(AudioClip audioClip, bool reuseAudioSourceIfAlreadyPlaying = true, bool isUISound = false, bool loop = false)
	{
		if (audioClip != null && (!UIOnly || isUISound))
		{
			if (reuseAudioSourceIfAlreadyPlaying)
			{
				foreach (AudioSource a in unavailableAudioSources)
				{
					if (a.clip == audioClip)
					{
						if (a.isPlaying) a.Stop();
						a.loop = loop;
						a.Play();
						return;
					}
				}
			}
			
			if (availableAudioSources.Count > 0)
			{
				AudioSource a = availableAudioSources[availableAudioSources.Count-1];
				availableAudioSources.Remove(a);
				unavailableAudioSources.Add(a);
				if (a.isPlaying) a.Stop();
				a.clip = audioClip;
				a.loop = loop;
				a.Play();
			}
		}
	}
	
	
	public static void StopSFX(AudioClip audioClip)
	{
		if (AudioManager.Main != null && audioClip != null)
		{
			AudioManager.Main.stopSFX(audioClip);
		}
	}
	
	public void stopSFX(AudioClip audioClip)
	{
		if (audioClip != null)
		{
			foreach (AudioSource a in unavailableAudioSources)
			{
				if (a.clip == audioClip)
				{
					if (a.isPlaying) a.Stop();
				}
			}
		}
	}
	
	public static void StopLoopingSFX(AudioClip audioClip)
	{
		if (AudioManager.Main != null && audioClip != null)
		{
			AudioManager.Main.stopLoopingSFX(audioClip);
		}
	}
	
	public void stopLoopingSFX(AudioClip audioClip)
	{
		if (audioClip != null)
		{
			foreach (AudioSource a in unavailableAudioSources)
			{
				if (a.clip == audioClip)
				{
					if (a.isPlaying && a.loop == true) a.loop = false;
				}
			}
		}
	}
	
}
