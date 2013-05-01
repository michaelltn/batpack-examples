using UnityEngine;
using System.Collections;

public abstract class LevelResults : MonoBehaviour
{
	public bool initDuringStart = false;
	
	protected bool showing;
	public bool isShowing { get { return showing; } }
	
	public delegate void resultsDisplayedDelegate();
	public event resultsDisplayedDelegate onResultsDisplayed;
	protected void resultsDisplayedEvent()
	{
		if (onResultsDisplayed != null)
			onResultsDisplayed();
	}
	
	public bool fruitBadgeEarned(FruitEater fruitEater)
	{
		if (fruitEater && fruitEater.fruitsEaten >= Fruit.TotalFruit)
			return true;
		return false;
	}
	
	public bool timeBadgeEarned(LevelTimer levelTimer)
	{
		if (levelTimer && levelTimer.time <= Level.TimeGoal)
			return true;
		return false;
	}
	
	public bool flapBadgeEarned(FlapCounter flapCounter)
	{
		return flapCounter.flapCount <= Level.FlapGoal;
	}
	
	public bool newBestTime(LevelTimer levelTimer)
	{
		if (levelTimer)
		{
			if (ProgressManager.LevelHasQuickestTime(Application.loadedLevelName))
			{
				float bestTime = ProgressManager.GetQuickestTime(Application.loadedLevelName);
				float currentTime = ProgressManager.RoundTime(levelTimer.time);
				return currentTime < bestTime;
			}
			return true;
		}
		return false;
	}
	
	public void saveResults(FlapCounter flapCounter, FruitEater fruitEater, LevelTimer levelTimer)
	{
		int numBadges = 0;
		if (flapCounter && flapBadgeEarned(flapCounter)) numBadges++;
		if (fruitEater && fruitBadgeEarned(fruitEater)) numBadges++;
		if (levelTimer && timeBadgeEarned(levelTimer)) numBadges++;
		
		BadgeCollectionManager.SetLevelBadges(Application.loadedLevelName, numBadges);
		if (newBestTime(levelTimer))
		{
			float currentTime = ProgressManager.RoundTime(levelTimer.time);
			ProgressManager.SetQuickestTime(Application.loadedLevelName, currentTime);
		}
	}
	
	virtual public void init() {}
	virtual public void showResults(FlapCounter flapCounter, FruitEater fruitEater, LevelTimer levelTimer) {}
	
	
	
	void Start ()
	{
		if (initDuringStart)
			init();
	}
}
