using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
	private static GameState main = null;
	public static GameState Main { get { return main; } }
	
	public bool useTimeScaleForPause = true;
	
	private float _timeScale = 1f;
	public float timeScale
	{
		get { return _timeScale; }
		set
		{
			_timeScale = Mathf.Clamp01(value);
			if (!paused)
			{
				Time.timeScale = _timeScale;
			}
		}
	}
	
	private bool paused = false;
	public bool isPaused { get { return paused; } }
	public static bool IsPaused { get { return (Main != null && Main.isPaused); } }
	
	private bool locked = false;
	public bool isLocked { get { return locked; } }
	public static bool IsLocked { get { return (Main != null && Main.isLocked); } }
	
	public delegate void PauseDelegate();
	public event PauseDelegate pauseEvent;
	public delegate void UnpauseDelegate();
	public event UnpauseDelegate unpauseEvent;
	
	
	public void lockState()
	{
		if (!locked)
			locked = true;
	}
	
	public void unlockState()
	{
		if (locked)
			locked = false;
	}
	
	public void pauseGame()
	{
		if (!locked && !paused)
		{
			paused = true;
			if (useTimeScaleForPause)
				Time.timeScale = 0;
			if (this.pauseEvent != null)
				pauseEvent();
		}
	}
	
	public void unpauseGame()
	{
		if (!locked && paused)
		{
			paused = false;
			if (useTimeScaleForPause)
				Time.timeScale = timeScale;
			if (this.unpauseEvent != null)
				unpauseEvent();
		}
	}
	
	
	void Awake ()
	{
		if (main == null)
			main = this;
		else
			Destroy(this.gameObject);
	}
	
	void OnDestroy ()
	{
		if (main == this)
		{
			if (useTimeScaleForPause)
				Time.timeScale = timeScale;
			main = null;
		}
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (paused)
				unpauseGame();
			else
				pauseGame();
		}
	}
	
	void OnApplicationFocus (bool focus)
	{
		if (!focus)
			pauseGame();
	}
	
	void OnApplicationPause (bool pause)
	{
		if (pause)
			pauseGame();
		//else
		//	unpauseGame();
	}
}
