using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class MusicTrack : MonoBehaviour
{
	private float _targetVolume = 1f;
	public float targetVolume
	{
		get { return _targetVolume; }
		set { _targetVolume = Mathf.Clamp01(value); }
	}
	
	private float fadeSpeed = 1f;
	
	public void fadeOut(float fadeTime)
	{
		fadeTo(0, fadeTime);
	}
	
	public void fadeTo(float targetVolume, float fadeTime)
	{
		if (fadeTime > 0)
		{
			this.targetVolume = targetVolume;
			fadeSpeed =  Mathf.Abs(this.volume - targetVolume) / fadeTime;
		}
	}
	
	
	public void dontDestroy()
	{
		DontDestroyOnLoad(this.gameObject);	
	}
	
	void Start ()
	{
		this.audio.loop = true;
	}

	
	float volumeDifference;
	void Update ()
	{
		this.audio.mute = !GameSettings.MusicIsOn;
		
		if (targetVolume != this.audio.volume)
		{
			volumeDifference = this.audio.volume - targetVolume;
			if (Mathf.Abs(volumeDifference) > fadeSpeed * Time.deltaTime)
				volumeDifference = (fadeSpeed * Time.deltaTime) * volumeDifference / Mathf.Abs(volumeDifference);
				
			this.audio.volume -= volumeDifference;
		}
	}
	
	
	public float volume
	{
		get { return this.audio.volume; }
		set { this.targetVolume = this.audio.volume = Mathf.Clamp01(value); }
	}
	
	public void play()
	{
		this.audio.Play();
	}
	
	public void play(AudioClip audioClip)
	{
		if (this.audio.isPlaying) this.audio.Stop();
		this.audio.clip = audioClip;
		this.audio.Play();
	}
	
	public void pause()
	{
		this.audio.Pause();
	}
	
	public void stop()
	{
		this.audio.Stop();
	}
	
	public AudioClip clip
	{
		get { return this.audio.clip; }
	}
	
	public bool isPlaying
	{
		get { return this.audio.isPlaying; }
	}
	
}
