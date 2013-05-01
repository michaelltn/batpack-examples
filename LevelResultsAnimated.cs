using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelResultsAnimated : LevelResults
{
	AnimationQueue flapBadgeAnimationQueue;
	bool flapBadgeAnimationStarted, flapBadgeAnimationComplete;
	UIStateSprite flapBadgeSprite, flapBadgeGlowSprite;
	AudioClip flapBadgeAudio;
	
	AnimationQueue fruitBadgeAnimationQueue;
	bool fruitBadgeAnimationStarted, fruitBadgeAnimationComplete;
	UIStateSprite fruitBadgeSprite, fruitBadgeGlowSprite;
	AudioClip fruitBadgeAudio;
	
	AnimationQueue timeBadgeAnimationQueue;
	bool timeBadgeAnimationStarted, timeBadgeAnimationComplete;
	UIStateSprite timeBadgeSprite, timeBadgeGlowSprite;
	AudioClip timeBadgeAudio;
	
	bool badgeAnimationStarted, badgeAnimationSkipped;
	bool badgeAnimationComplete
	{
		get
		{
			return
				badgeAnimationStarted &&
				flapBadgeAnimationStarted == flapBadgeAnimationComplete &&
				fruitBadgeAnimationStarted == fruitBadgeAnimationComplete &&
				timeBadgeAnimationStarted == timeBadgeAnimationComplete;
		}
	}
	
	public const string HasSeenFlapAnimationKey = "HasSeenFlapAnimation";
	bool HasSeenFlapAnimation
	{
		get { return EncryptedPlayerPrefs.GetInt(HasSeenFlapAnimationKey, 0) == 1; }
		set { EncryptedPlayerPrefs.SetInt(HasSeenFlapAnimationKey, value ? 1 : 0); }
	}
	
	public const string HasSeenFruitAnimationKey = "HasSeenFruitAnimation";
	bool HasSeenFruitAnimation
	{
		get { return EncryptedPlayerPrefs.GetInt(HasSeenFruitAnimationKey, 0) == 1; }
		set { EncryptedPlayerPrefs.SetInt(HasSeenFruitAnimationKey, value ? 1 : 0); }
	}
	
	public const string HasSeenTimeAnimationKey = "HasSeenTimeAnimation";
	bool HasSeenTimeAnimation
	{
		get { return EncryptedPlayerPrefs.GetInt(HasSeenTimeAnimationKey, 0) == 1; }
		set { EncryptedPlayerPrefs.SetInt(HasSeenTimeAnimationKey, value ? 1 : 0); }
	}
	
	bool dontSkipBadgeAnimation
	{
		get
		{
			return
				(gotFlapBadge  && HasSeenFlapAnimation  == false) ||
				(gotFruitBadge && HasSeenFruitAnimation == false) ||
				(gotTimeBadge  && HasSeenTimeAnimation  == false);
		}
	}
	
	
	
	#region connection properties
	
	public GoldenFruitEarnedUI goldenFruitEarnedUI;
	public PauseMenu pauseMenu;
	public BadgeUI badgeUI;
	public NavigationButtons navigationButtons;
	//public EveryPlayButton everyPlayButton;
	public NextLevelDialog nextLevelDialog;
	
	#endregion
	
	#region UIToolKit Sprites
	
	public string bronzeBadgeAtlasReference = "badge_medal_bronze_noRest.png";
	public string silverBadgeAtlasReference = "badge_medal_silver_noRest.png";
	public string goldBadgeAtlasReference = "badge_medal_gold_noRest.png";
	private UIStateSprite[] bronzeBadgeSprites;
	private UIStateSprite[] silverBadgeSprites;
	private UIStateSprite[] goldBadgeSprites;
	
	//public string flapIconAtlasReference = "bp_gui_noRest_icon_glowing01.png";
	//public string fruitIconAtlasReference = "bp_gui_fruit_icon_glowing01.png";
	//public string timeIconAtlasReference = "bp_gui_time_icon_glowing.png";
	private UIStateSprite movingFlapIcon;
	private UIStateSprite movingFruitIcon;
	private UIStateSprite movingTimeIcon;
	
	public float previousBadgeScale = 0.8f;
	
	public string badgeGlowEffectAtlasReference = "glowEffect.png";
	private UIStateSprite[] badgeGlowEffectSprites;
	public float glowRotationSpeed = 36f;
	
	public int horizontalBuffer = 32;
	public int verticalBuffer = 16;
	
	#endregion
	
	#region Sound Effects
	
	public AudioClip[] badgeSFX;
	
	#endregion

	
	
	float heightFactor;
	float widthFactor;
	public override void init ()
	{
		flapBadgeAnimationQueue = new AnimationQueue();
		flapBadgeAnimationStarted = flapBadgeAnimationComplete = false;
		fruitBadgeAnimationQueue = new AnimationQueue();
		fruitBadgeAnimationStarted = fruitBadgeAnimationComplete = false;
		timeBadgeAnimationQueue = new AnimationQueue();
		timeBadgeAnimationStarted = timeBadgeAnimationComplete = false;
		badgeAnimationStarted = badgeAnimationSkipped = false;
		
		heightFactor = 1f/Screen.height;
		widthFactor = 1f/Screen.width;
		
		horizontalBuffer *= UI.scaleFactor;
		verticalBuffer *=  UI.scaleFactor;
		
		int i;
		
		bronzeBadgeSprites = new UIStateSprite[1];
		silverBadgeSprites = new UIStateSprite[2];
		goldBadgeSprites = new UIStateSprite[3];
		badgeGlowEffectSprites = new UIStateSprite[3];
		
		for (i = 0; i < 3; i++)
		{
			if (i < badgeGlowEffectSprites.Length)
				badgeGlowEffectSprites[i] = UIStateSprite.create(badgeGlowEffectAtlasReference, 0, 0);
			if (i < bronzeBadgeSprites.Length)
				bronzeBadgeSprites[i] = UIStateSprite.create(bronzeBadgeAtlasReference, 0, 0);
			if (i < silverBadgeSprites.Length)
				silverBadgeSprites[i] = UIStateSprite.create(silverBadgeAtlasReference, 0, 0);
			if (i < goldBadgeSprites.Length)
				goldBadgeSprites[i] = UIStateSprite.create(goldBadgeAtlasReference, 0, 0);
		}
		
		float y = (Screen.height/2f) - (badgeGlowEffectSprites[0].width/2f) - verticalBuffer;
		float[] x = new float[3];
		x[0] = -badgeGlowEffectSprites[0].width - horizontalBuffer;
		x[1] = 0;
		x[2] = badgeGlowEffectSprites[0].width + horizontalBuffer;
		
		for (i = 0; i < 3; i++)
		{
			if (i < badgeGlowEffectSprites.Length)
			{
				badgeGlowEffectSprites[i].positionFromCenter(y * heightFactor, x[i] * widthFactor);
				badgeGlowEffectSprites[i].hidden = true;
				badgeGlowEffectSprites[i].centerize();
			}
			if (i < bronzeBadgeSprites.Length)
			{
				bronzeBadgeSprites[i].positionFromCenter(y * heightFactor, x[i] * widthFactor);
				bronzeBadgeSprites[i].hidden = true;
				bronzeBadgeSprites[i].centerize();
			}
			if (i < silverBadgeSprites.Length)
			{
				silverBadgeSprites[i].positionFromCenter(y * heightFactor, x[i] * widthFactor);
				silverBadgeSprites[i].hidden = true;
				silverBadgeSprites[i].centerize();
			}
			if (i < goldBadgeSprites.Length)
			{
				goldBadgeSprites[i].positionFromCenter(y * heightFactor, x[i] * widthFactor);
				goldBadgeSprites[i].hidden = true;
				goldBadgeSprites[i].centerize();
			}
		}
		
		if (Everyplay.SharedInstance != null && Everyplay.SharedInstance.IsSupported())
		{
			if (TransitionLoader.Main)
			{
				TransitionLoader.Main.onFadeOutBegin += endEveryPlayRecording;
			}
			if (Everyplay.SharedInstance.IsRecording())
				Everyplay.SharedInstance.StopRecording();
			Everyplay.SharedInstance.StartRecording();
		}
	}
	
	bool gotFruitBadge = false, gotTimeBadge = false, gotFlapBadge = false;
	private int fruitCount = 0;
	private int badgeCount = 0;
	private int nextBadge;
	private int newBadgesEarned;
	private int goldenFruitEarned;
	private int startingGoldenFruit;
	public override void showResults(FlapCounter flapCounter, FruitEater fruitEater, LevelTimer levelTimer)
	{
		if (showing) return;
		showing = true;
		
		AudioManager.UIOnly = true;
		MusicManager.fadeTo(0.2f, 1f);
		
		badgeCount = 0;
		fruitCount = 0;
		goldenFruitEarned = 0;
		startingGoldenFruit = GoldenFruit.Available;
		
		if (flapCounter && levelTimer && fruitEater)
		{
			fruitCount = fruitEater.fruitsEaten;
			if (fruitCount > 0)
			{
				goldenFruitEarned += fruitCount;
			}
			
			if (flapBadgeEarned(flapCounter))
			{
				gotFlapBadge = true;
				badgeCount++;
			}
			
			if (fruitBadgeEarned(fruitEater))
			{
				gotFruitBadge = true;
				badgeCount++;
			}
			
			if (timeBadgeEarned(levelTimer))
			{
				gotTimeBadge = true;
				badgeCount++;
			}
			
			if (!levelTimer.animateNewBestTime())
				levelTimer.show();
			
			badgeAnimationStarted = true;
			
			nextBadge = 0;
			if (gotFlapBadge)
			{
				flapBadgeAnimationStarted = true;
				
				flapBadgeSprite = null;
				switch (badgeCount)
				{
				case 1:
					if (nextBadge < bronzeBadgeSprites.Length)
						flapBadgeSprite = bronzeBadgeSprites[nextBadge];
					break;
				case 2:
					if (nextBadge < silverBadgeSprites.Length)
						flapBadgeSprite = silverBadgeSprites[nextBadge];
					break;
				case 3:
					if (nextBadge < goldBadgeSprites.Length)
						flapBadgeSprite = goldBadgeSprites[nextBadge];
					break;
				}
				
				flapBadgeGlowSprite = null;
				if (nextBadge < badgeGlowEffectSprites.Length)
					flapBadgeGlowSprite = badgeGlowEffectSprites[nextBadge];
				
				flapBadgeAudio = null;
				if (nextBadge < badgeSFX.Length)
					flapBadgeAudio = badgeSFX[nextBadge];
				
				nextBadge++;
				
				flapBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startFlapIconMovement, updateFlapIconMovement, skipFlapBadgeAnimation));
				flapBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startFlapBadgeAnimateIn, updateFlapBadgeAnimateIn, skipFlapBadgeAnimation));
				
				flapBadgeAnimationStarted = true;
			}
			
			if (gotFruitBadge)
			{
				fruitBadgeAnimationStarted = true;
				
				fruitBadgeSprite = null;
				switch (badgeCount)
				{
				case 1:
					if (nextBadge < bronzeBadgeSprites.Length)
						fruitBadgeSprite = bronzeBadgeSprites[nextBadge];
					break;
				case 2:
					if (nextBadge < silverBadgeSprites.Length)
						fruitBadgeSprite = silverBadgeSprites[nextBadge];
					break;
				case 3:
					if (nextBadge < goldBadgeSprites.Length)
						fruitBadgeSprite = goldBadgeSprites[nextBadge];
					break;
				}
				
				fruitBadgeGlowSprite = null;
				if (nextBadge < badgeGlowEffectSprites.Length)
					fruitBadgeGlowSprite = badgeGlowEffectSprites[nextBadge];
				
				fruitBadgeAudio = null;
				if (nextBadge < badgeSFX.Length)
					fruitBadgeAudio = badgeSFX[nextBadge];
				
				nextBadge++;
				
				fruitBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startWaitingForFlapAnimation, updateWaitingForFlapAnimation, skipFruitBadgeAnimation));
				fruitBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startFruitIconMovement, updateFruitIconMovement, skipFruitBadgeAnimation));
				fruitBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startFruitBadgeAnimateIn, updateFruitBadgeAnimateIn, skipFruitBadgeAnimation));
				
				fruitBadgeAnimationStarted = true;
			}
			
			if (gotTimeBadge)
			{
				timeBadgeAnimationStarted = true;
				
				timeBadgeSprite = null;
				switch (badgeCount)
				{
				case 1:
					if (nextBadge < bronzeBadgeSprites.Length)
						timeBadgeSprite = bronzeBadgeSprites[nextBadge];
					break;
				case 2:
					if (nextBadge < silverBadgeSprites.Length)
						timeBadgeSprite = silverBadgeSprites[nextBadge];
					break;
				case 3:
					if (nextBadge < goldBadgeSprites.Length)
						timeBadgeSprite = goldBadgeSprites[nextBadge];
					break;
				}
				
				timeBadgeGlowSprite = null;
				if (nextBadge < badgeGlowEffectSprites.Length)
					timeBadgeGlowSprite = badgeGlowEffectSprites[nextBadge];
				
				timeBadgeAudio = null;
				if (nextBadge < badgeSFX.Length)
					timeBadgeAudio = badgeSFX[nextBadge];
				
				nextBadge++;
				
				timeBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startWaitingForFruitAnimation, updateWaitingForFruitAnimation, skipTimeBadgeAnimation));
				timeBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startTimeIconMovement, updateTimeIconMovement, skipTimeBadgeAnimation));
				timeBadgeAnimationQueue.addAnimationStep(
					new AnimationStep(startTimeBadgeAnimateIn, updateTimeBadgeAnimateIn, skipTimeBadgeAnimation));
				
				timeBadgeAnimationStarted = true;
			}
			
			newBadgesEarned = badgeCount - BadgeCollectionManager.GetLevelBadges(Application.loadedLevelName);
			if (newBadgesEarned > 0)
			{
				goldenFruitEarned += newBadgesEarned * 25;
			}
			
			GoldenFruit.Collect(goldenFruitEarned);
			
			saveResults(flapCounter, fruitEater, levelTimer);
			
			
			nextLevelDialog.onNextLevelButtonTouched += nextButtonTouched;
			if (goldenFruitEarnedUI != null)
			{
				goldenFruitEarnedUI.setResultsValues(startingGoldenFruit, fruitCount, badgeCount);
				goldenFruitEarnedUI.onAnimationComplete += fadeInNavigationButtons;
				goldenFruitEarnedUI.onAnimationSkipped += showNavigationButtons;
				goldenFruitEarnedUI.onAnimationComplete += fadeInNextLevelDialog;
				goldenFruitEarnedUI.onAnimationSkipped += showNextLevelDialog;
				
				if (Everyplay.SharedInstance != null && Everyplay.SharedInstance.IsSupported())
				{
					goldenFruitEarnedUI.onAnimationComplete += endEveryPlayRecording;
					goldenFruitEarnedUI.onAnimationSkipped += endEveryPlayRecording;
				}
			}
			else
			{
				showNavigationButtons();
				showNextLevelDialog();
			}
			
			flapBadgeAnimationQueue.onAnimationQueueComplete += flapAnimationQueueComplete;
			fruitBadgeAnimationQueue.onAnimationQueueComplete += fruitAnimationQueueComplete;
			timeBadgeAnimationQueue.onAnimationQueueComplete += timeAnimationQueueComplete;
			
			flapBadgeAnimationQueue.beginNextAnimation();
			fruitBadgeAnimationQueue.beginNextAnimation();
			timeBadgeAnimationQueue.beginNextAnimation();
		}
	}
	

	
	void endEveryPlayRecording()
	{
		if (Everyplay.SharedInstance.IsRecording())
		{
			Everyplay.SharedInstance.StopRecording();
		}
	}
	
	void fadeInNextLevelDialog()
	{
		if (nextLevelDialog != null)
			nextLevelDialog.fadeIn(0.67f);
		
		int i;
		for (i = 0; i < badgeGlowEffectSprites.Length; i++)
		{
			badgeGlowEffectSprites[i].alphaFromTo(1f, 1f, 0, Easing.Sinusoidal.easeOut);
		}
		for (i = 0; i < bronzeBadgeSprites.Length; i++)
		{
			bronzeBadgeSprites[i].alphaFromTo(1f, 1f, 0, Easing.Sinusoidal.easeOut);
		}
		for (i = 0; i < silverBadgeSprites.Length; i++)
		{
			silverBadgeSprites[i].alphaFromTo(1f, 1f, 0, Easing.Sinusoidal.easeOut);
		}
		for (i = 0; i < goldBadgeSprites.Length; i++)
		{
			goldBadgeSprites[i].alphaFromTo(1f, 1f, 0, Easing.Sinusoidal.easeOut);
		}
	}
	
	void showNextLevelDialog()
	{
		if (nextLevelDialog != null)
			nextLevelDialog.show();
		
		int i;
		for (i = 0; i < badgeGlowEffectSprites.Length; i++)
		{
			badgeGlowEffectSprites[i].hidden = true;
		}
		for (i = 0; i < bronzeBadgeSprites.Length; i++)
		{
			bronzeBadgeSprites[i].hidden = true;
		}
		for (i = 0; i < silverBadgeSprites.Length; i++)
		{
			silverBadgeSprites[i].hidden = true;
		}
		for (i = 0; i < goldBadgeSprites.Length; i++)
		{
			goldBadgeSprites[i].hidden = true;
		}
	}
	
		
	void Update ()
	{
		Vector3 newRotation;
		for (int i = 0; i < 3; i++)
		{
			if (badgeGlowEffectSprites[i] != null)
			{
				newRotation = badgeGlowEffectSprites[i].eulerAngles;
				newRotation.z -= glowRotationSpeed * Time.deltaTime;
				if (newRotation.z < 0) newRotation.z += 360.0f;
				badgeGlowEffectSprites[i].eulerAngles = newRotation;
			}
		}
		
		flapBadgeAnimationQueue.updateAnimation();
		fruitBadgeAnimationQueue.updateAnimation();
		timeBadgeAnimationQueue.updateAnimation();
	}
	
	
	#region Badge Bounce Animation
	
	const float badgeBounceTime = 0.28f;
	const float badgeBounceScale = 0.8f;
	
	void beginBadgeBounce(UIStateSprite sprite)
	{
		UIAnimation newAnim = sprite.scaleFromTo(
			badgeBounceTime,
			new Vector3(badgeBounceScale, badgeBounceScale, badgeBounceScale),
			new Vector3(1.0f, 1.0f, 1.0f),
			Easing.Sinusoidal.easeOut);
		newAnim.autoreverse  = true;
		newAnim.onCompleteEvent += badgeBounceComplete;
	}
	void badgeBounceLoop(UIStateSprite sprite)
	{
		UIAnimation newAnim = sprite.scaleFromTo(
			badgeBounceTime,
			new Vector3(badgeBounceScale, badgeBounceScale, badgeBounceScale),
			new Vector3(1.0f, 1.0f, 1.0f),
			Easing.Sinusoidal.easeInOut);
		newAnim.autoreverse  = true;
		newAnim.onCompleteEvent += badgeBounceComplete;
	}
	void badgeBounceComplete(UIAnimation anim)
	{
		UIStateSprite sprite = (anim.getSprite() as UIStateSprite);
		if (sprite != null)
		{
			badgeBounceLoop(sprite);
		}
	}
	
	#endregion

	
	
	
	const float iconMovementTime = 1f;
	const float badgeAnimateInTime = 0.33f;

	const float iconLargeScale = 2f;
	const float iconNormalScale = 1f;
	const float iconSmallScale = 0.25f;
	
	float stepScale;
	Vector3 stepPosition;
	
	#region Flap Badge AnimationQueue functions
	
	float flapStepTime, flapStepTimeRemaining;
	private float flapStepProgress { get { return Mathf.Clamp01(1f - (flapStepTimeRemaining / flapStepTime)); } }
	private bool flapStepComplete { get { return (flapStepTimeRemaining <= 0); } }

	void startFlapIconMovement()
	{
		flapStepTimeRemaining = flapStepTime = iconMovementTime;
		
		movingFlapIcon = UIStateSprite.create(badgeUI.flapBadgeIcon.glowIconReferenceName, 0, 0);
		movingFlapIcon.centerize();
		movingFlapIcon.position = badgeUI.flapBadgePosition;
		UIToolkitHelper.SetScale(movingFlapIcon, iconNormalScale);
	}
	void updateFlapIconMovement()
	{
		flapStepTimeRemaining -= Time.deltaTime;
		
		if (flapStepProgress < 0.75f)
		{
			stepScale =
				(iconLargeScale - iconNormalScale)
				* Easing.Sinusoidal.easeOut(flapStepProgress / 0.75f)
				+ iconNormalScale;
		}
		else
		{
			stepScale =
				(iconNormalScale - iconLargeScale)
				* Easing.Sinusoidal.easeIn((flapStepProgress - 0.75f) / 0.25f)
				+ iconLargeScale;
		}
		UIToolkitHelper.SetScale(movingFlapIcon, stepScale);
		
		movingFlapIcon.position =
			(flapBadgeSprite.position - badgeUI.flapBadgePosition)
			* Easing.Sinusoidal.easeInOut(flapStepProgress)
			+ badgeUI.flapBadgePosition;
		
		if (flapStepComplete)
		{
			flapBadgeAnimationQueue.beginNextAnimation();
			fruitBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	void startFlapBadgeAnimateIn()
	{
		flapStepTimeRemaining = flapStepTime = badgeAnimateInTime;
		
		flapBadgeGlowSprite.hidden = false;
		UIToolkitHelper.SetAlpha(flapBadgeGlowSprite, 0);
		UIToolkitHelper.SetScale(flapBadgeGlowSprite, 0);

		flapBadgeSprite.hidden = false;
		UIToolkitHelper.SetAlpha(flapBadgeSprite, 0);
		UIToolkitHelper.SetScale(flapBadgeSprite, 0);

		AudioManager.PlaySFX(flapBadgeAudio, false, true);
	}
	void updateFlapBadgeAnimateIn()
	{
		flapStepTimeRemaining -= Time.deltaTime;
		
		stepScale =
			(iconSmallScale - iconNormalScale)
			* Easing.Sinusoidal.easeOut(flapStepProgress)
			+ iconNormalScale;
		UIToolkitHelper.SetScale(movingFlapIcon, stepScale);
		UIToolkitHelper.SetAlpha(movingFlapIcon, Easing.Sinusoidal.easeOut(1f - flapStepProgress));
		if (movingFlapIcon.position != flapBadgeSprite.position)
			movingFlapIcon.position = flapBadgeSprite.position;
		
		UIToolkitHelper.SetAlpha(flapBadgeGlowSprite, flapStepProgress);
		UIToolkitHelper.SetScale(flapBadgeGlowSprite, 1.5f * Easing.Sinusoidal.easeOut(flapStepProgress));
		
		UIToolkitHelper.SetAlpha(flapBadgeSprite, flapStepProgress);
		UIToolkitHelper.SetScale(flapBadgeSprite, badgeBounceScale * Easing.Sinusoidal.easeIn(flapStepProgress));
		
		if (flapStepComplete)
		{
			if (movingFlapIcon != null)
				movingFlapIcon.hidden = true;
				
			flapBadgeAnimationComplete = true;
			beginBadgeBounce(flapBadgeSprite);
			flapBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	AnimationStep.SkipResult skipFlapBadgeAnimation()
	{
		if (dontSkipBadgeAnimation)
			return AnimationStep.SkipResult.DontSkip;
		
		badgeAnimationSkipped = true;
		flapBadgeAnimationComplete = true;
		
		if (movingFlapIcon != null)
			movingFlapIcon.hidden = true;
		
		flapBadgeGlowSprite.hidden = false;
		UIToolkitHelper.SetAlpha(flapBadgeGlowSprite, 1f);
		UIToolkitHelper.SetScale(flapBadgeGlowSprite, 1.5f);

		flapBadgeSprite.hidden = false;
		UIToolkitHelper.SetAlpha(flapBadgeSprite, 1f);
		UIToolkitHelper.SetScale(flapBadgeSprite, badgeBounceScale);
		
		beginBadgeBounce(flapBadgeSprite);
		return AnimationStep.SkipResult.SkipAll;
	}

	void flapAnimationQueueComplete(AnimationQueue animationQueue)
	{
		flapBadgeAnimationQueue.onAnimationQueueComplete -= flapAnimationQueueComplete;
		if (flapBadgeAnimationStarted == true)
			flapBadgeAnimationComplete = true;
			
		if (fruitBadgeAnimationStarted == false && badgeAnimationSkipped == false)
		{
			fruitBadgeAnimationQueue.beginNextAnimation();
		}
	}

	#endregion
	
	
	
	#region Fruit Badge AnimationQueue functions
	
	float fruitStepTime, fruitStepTimeRemaining;
	private float fruitStepProgress { get { return Mathf.Clamp01(1f - (fruitStepTimeRemaining / fruitStepTime)); } }
	private bool fruitStepComplete { get { return (fruitStepTimeRemaining <= 0); } }
	
	void startWaitingForFlapAnimation() {}
	void updateWaitingForFlapAnimation()
	{
		if (flapBadgeAnimationStarted == flapBadgeAnimationComplete)
			fruitBadgeAnimationQueue.beginNextAnimation();
	}
	
	void startFruitIconMovement()
	{
		fruitStepTimeRemaining = fruitStepTime = iconMovementTime;
		
		movingFruitIcon = UIStateSprite.create(badgeUI.fruitBadgeIcon.glowIconReferenceName, 0, 0);
		movingFruitIcon.centerize();
		movingFruitIcon.position = badgeUI.fruitBadgePosition;
		UIToolkitHelper.SetScale(movingFruitIcon, iconNormalScale);
	}
	void updateFruitIconMovement()
	{
		fruitStepTimeRemaining -= Time.deltaTime;
		
		if (fruitStepProgress < 0.75f)
		{
			stepScale =
				(iconLargeScale - iconNormalScale)
				* Easing.Sinusoidal.easeOut(fruitStepProgress / 0.75f)
				+ iconNormalScale;
		}
		else
		{
			stepScale =
				(iconNormalScale - iconLargeScale)
				* Easing.Sinusoidal.easeIn((fruitStepProgress - 0.75f) / 0.25f)
				+ iconLargeScale;
		}
		UIToolkitHelper.SetScale(movingFruitIcon, stepScale);
		
		movingFruitIcon.position =
			(fruitBadgeSprite.position - badgeUI.fruitBadgePosition)
			* Easing.Sinusoidal.easeInOut(fruitStepProgress)
			+ badgeUI.fruitBadgePosition;
		
		if (fruitStepComplete)
		{
			fruitBadgeAnimationQueue.beginNextAnimation();
			timeBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	void startFruitBadgeAnimateIn()
	{
		fruitStepTimeRemaining = fruitStepTime = badgeAnimateInTime;
		
		fruitBadgeGlowSprite.hidden = false;
		UIToolkitHelper.SetAlpha(fruitBadgeGlowSprite, 0);
		UIToolkitHelper.SetScale(fruitBadgeGlowSprite, 0);

		fruitBadgeSprite.hidden = false;
		UIToolkitHelper.SetAlpha(fruitBadgeSprite, 0);
		UIToolkitHelper.SetScale(fruitBadgeSprite, 0);

		AudioManager.PlaySFX(fruitBadgeAudio, false, true);
	}
	void updateFruitBadgeAnimateIn()
	{
		fruitStepTimeRemaining -= Time.deltaTime;
		
		stepScale =
			(iconSmallScale - iconNormalScale)
			* Easing.Sinusoidal.easeOut(fruitStepProgress)
			+ iconNormalScale;
		UIToolkitHelper.SetScale(movingFruitIcon, stepScale);
		UIToolkitHelper.SetAlpha(movingFruitIcon, Easing.Sinusoidal.easeOut(1f - fruitStepProgress));
		if (movingFruitIcon.position != fruitBadgeSprite.position)
			movingFruitIcon.position = fruitBadgeSprite.position;
		
		UIToolkitHelper.SetAlpha(fruitBadgeGlowSprite, fruitStepProgress);
		UIToolkitHelper.SetScale(fruitBadgeGlowSprite, 1.5f * Easing.Sinusoidal.easeOut(fruitStepProgress));
	
		UIToolkitHelper.SetAlpha(fruitBadgeSprite, fruitStepProgress);
		UIToolkitHelper.SetScale(fruitBadgeSprite, badgeBounceScale * Easing.Sinusoidal.easeIn(fruitStepProgress));
	
		if (fruitStepComplete)
		{
			if (movingFruitIcon != null)
				movingFruitIcon.hidden = true;
				
			fruitBadgeAnimationComplete = true;
			beginBadgeBounce(fruitBadgeSprite);
			fruitBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	AnimationStep.SkipResult skipFruitBadgeAnimation()
	{
		if (dontSkipBadgeAnimation)
			return AnimationStep.SkipResult.DontSkip;
		
		badgeAnimationSkipped = true;
		fruitBadgeAnimationComplete = true;
		
		if (movingFruitIcon != null)
			movingFruitIcon.hidden = true;
		
		fruitBadgeGlowSprite.hidden = false;
		UIToolkitHelper.SetAlpha(fruitBadgeGlowSprite, 1f);
		UIToolkitHelper.SetScale(fruitBadgeGlowSprite, 1.5f);
	
		fruitBadgeSprite.hidden = false;
		UIToolkitHelper.SetAlpha(fruitBadgeSprite, 1f);
		UIToolkitHelper.SetScale(fruitBadgeSprite, badgeBounceScale);
		
		beginBadgeBounce(fruitBadgeSprite);
		return AnimationStep.SkipResult.SkipAll;
	}

	void fruitAnimationQueueComplete(AnimationQueue animationQueue)
	{
		fruitBadgeAnimationQueue.onAnimationQueueComplete -= fruitAnimationQueueComplete;
		if (fruitBadgeAnimationStarted == true)
			fruitBadgeAnimationComplete = true;
		
		if (timeBadgeAnimationStarted == false && badgeAnimationSkipped == false)
		{
			timeBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	#endregion
	
	
	
	#region Time Badge AnimationQueue functions
	
	float timeStepTime, timeStepTimeRemaining;
	private float timeStepProgress { get { return Mathf.Clamp01(1f - (timeStepTimeRemaining / timeStepTime)); } }
	private bool timeStepComplete { get { return (timeStepTimeRemaining <= 0); } }

	void startWaitingForFruitAnimation() {}
	void updateWaitingForFruitAnimation()
	{
		if (flapBadgeAnimationStarted == flapBadgeAnimationComplete &&
			fruitBadgeAnimationStarted == fruitBadgeAnimationComplete)
		{
			timeBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	void startTimeIconMovement()
	{
		timeStepTimeRemaining = timeStepTime = iconMovementTime;
		
		movingTimeIcon = UIStateSprite.create(badgeUI.timeBadgeIcon.glowIconReferenceName, 0, 0);
		movingTimeIcon.centerize();
		movingTimeIcon.position = badgeUI.timeBadgePosition;
		UIToolkitHelper.SetScale(movingTimeIcon, iconNormalScale);
	}
	void updateTimeIconMovement()
	{
		timeStepTimeRemaining -= Time.deltaTime;
		
		if (timeStepProgress < 0.75f)
		{
			stepScale =
				(iconLargeScale - iconNormalScale)
				* Easing.Sinusoidal.easeOut(timeStepProgress / 0.75f)
				+ iconNormalScale;
		}
		else
		{
			stepScale =
				(iconNormalScale - iconLargeScale)
				* Easing.Sinusoidal.easeIn((timeStepProgress - 0.75f) / 0.25f)
				+ iconLargeScale;
		}
		UIToolkitHelper.SetScale(movingTimeIcon, stepScale);
		
		movingTimeIcon.position =
			(timeBadgeSprite.position - badgeUI.timeBadgePosition)
			* Easing.Sinusoidal.easeInOut(timeStepProgress)
			+ badgeUI.timeBadgePosition;
		
		if (timeStepComplete)
		{
			timeBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	void startTimeBadgeAnimateIn()
	{
		timeStepTimeRemaining = timeStepTime = badgeAnimateInTime;
		
		timeBadgeGlowSprite.hidden = false;
		UIToolkitHelper.SetAlpha(timeBadgeGlowSprite, 0);
		UIToolkitHelper.SetScale(timeBadgeGlowSprite, 0);
		
		timeBadgeSprite.hidden = false;
		UIToolkitHelper.SetAlpha(timeBadgeSprite, 0);
		UIToolkitHelper.SetScale(timeBadgeSprite, 0);

		AudioManager.PlaySFX(timeBadgeAudio, false, true);
	}
	void updateTimeBadgeAnimateIn()
	{
		timeStepTimeRemaining -= Time.deltaTime;
		
		stepScale =
			(iconSmallScale - iconNormalScale)
			* Easing.Sinusoidal.easeOut(timeStepProgress)
			+ iconNormalScale;
		UIToolkitHelper.SetScale(movingTimeIcon, stepScale);
		UIToolkitHelper.SetAlpha(movingTimeIcon, Easing.Sinusoidal.easeOut(1f - timeStepProgress));
		if (movingTimeIcon.position != timeBadgeSprite.position)
			movingTimeIcon.position = timeBadgeSprite.position;
	
		UIToolkitHelper.SetAlpha(timeBadgeGlowSprite, timeStepProgress);
		UIToolkitHelper.SetScale(timeBadgeGlowSprite, 1.5f * Easing.Sinusoidal.easeOut(timeStepProgress));
	
		UIToolkitHelper.SetAlpha(timeBadgeSprite, timeStepProgress);
		UIToolkitHelper.SetScale(timeBadgeSprite, badgeBounceScale * Easing.Sinusoidal.easeIn(timeStepProgress));
	
		if (timeStepComplete)
		{
			if (movingTimeIcon != null)
				movingTimeIcon.hidden = true;
			
			timeBadgeAnimationComplete = true;
			beginBadgeBounce(timeBadgeSprite);
			timeBadgeAnimationQueue.beginNextAnimation();
		}
	}
	
	AnimationStep.SkipResult skipTimeBadgeAnimation()
	{
		if (dontSkipBadgeAnimation)
			return AnimationStep.SkipResult.DontSkip;
		
		badgeAnimationSkipped = true;
		timeBadgeAnimationComplete = true;
		
		if (movingTimeIcon != null)
			movingTimeIcon.hidden = true;
		
		timeBadgeGlowSprite.hidden = false;
		UIToolkitHelper.SetAlpha(timeBadgeGlowSprite, 1f);
		UIToolkitHelper.SetScale(timeBadgeGlowSprite, 1.5f);
	
		timeBadgeSprite.hidden = false;
		UIToolkitHelper.SetAlpha(timeBadgeSprite, 1f);
		UIToolkitHelper.SetScale(timeBadgeSprite, badgeBounceScale);
		
		beginBadgeBounce(timeBadgeSprite);
		return AnimationStep.SkipResult.SkipAll;
	}

	void timeAnimationQueueComplete(AnimationQueue animationQueue)
	{
		timeBadgeAnimationQueue.onAnimationQueueComplete -= timeAnimationQueueComplete;
		if (timeBadgeAnimationStarted == true)
			timeBadgeAnimationComplete = true;
		
		if (gotFlapBadge && HasSeenFlapAnimation == false)
			HasSeenFlapAnimation = true;
		if (gotFruitBadge && HasSeenFruitAnimation == false)
			HasSeenFruitAnimation = true;
		if (gotTimeBadge && HasSeenTimeAnimation == false)
			HasSeenTimeAnimation = true;
		
		resultsDisplayedEvent();
		
		MusicManager.fadeTo(1f, 1f);
		
		if (goldenFruitEarnedUI != null)
			goldenFruitEarnedUI.show();
	}
	
	#endregion
	
	
	
	#region Navigation Buttons
	
	private void showNavigationButtons()
	{
		navigationButtons.show();
		navigationButtons.restartButton.onTouchUpInside += restartButtonTouched;
		navigationButtons.menuButton.onTouchUpInside += menuButtonTouched;
	}
	
	private void fadeInNavigationButtons()
	{
		navigationButtons.fadeIn();
		navigationButtons.restartButton.onTouchUpInside += restartButtonTouched;
		navigationButtons.menuButton.onTouchUpInside += menuButtonTouched;
	}
	
	private void hideNavigationButtons()
	{
		navigationButtons.fadeOut();
		navigationButtons.restartButton.onTouchUpInside -= restartButtonTouched;
		navigationButtons.menuButton.onTouchUpInside -= menuButtonTouched;
	}
	
	private void nextButtonTouched(UIButton sender)
	{
		if (badgeUI) badgeUI.fadeOut();
		hideNavigationButtons();
		
		string worldName = ProgressManager.GetWorldForLevel(Application.loadedLevelName);
		string nextLevel = ProgressManager.GetNextLevel(worldName, Application.loadedLevelName);
		if (nextLevel.Length > 0)
		{
			if (TransitionLoader.Main != null)
				TransitionLoader.Main.loadLevel(nextLevel);
			else
				Debug.LogError("No Transition Loader set.");
		}
		else
		{
			if (TransitionLoader.Main != null)
				TransitionLoader.Main.loadDefaultScene();
			else
				Debug.LogError("No Transition Loader set.");
		}
	}
	
	private void restartButtonTouched(UIButton sender)
	{
		if (badgeUI) badgeUI.fadeOut();
		hideNavigationButtons();
		if (TransitionLoader.Main != null)
			TransitionLoader.Main.loadLevel(Application.loadedLevelName);
		else
			Debug.LogError("No Transition Loader set.");
	}
	
	private void menuButtonTouched(UIButton sender)
	{
		if (badgeUI) badgeUI.fadeOut();
		hideNavigationButtons();
		if (TransitionLoader.Main != null)
		{
			MusicManager.fadeOut(TransitionLoader.Main.fadeOutTime);
			TransitionLoader.Main.loadDefaultScene();
		}
		else
			Debug.LogError("No Transition Loader set.");
	}
	
	#endregion
}