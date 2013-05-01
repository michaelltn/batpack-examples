using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationStep
{
	public delegate void AnimationDelegate();
	public delegate SkipResult SkipAnimationDelegate();
	
	public enum SkipResult { SkipOne, SkipAll, DontSkip };
	
	public AnimationStep(AnimationDelegate startDelegate)
	{
		if (startDelegate == null) throw new UnassignedReferenceException("startDelegate cannot be null");
		this.startDelegate = startDelegate;
	}
	public AnimationStep(AnimationDelegate startDelegate, AnimationDelegate updateDelegate)
	{
		if (startDelegate == null) throw new UnassignedReferenceException("startDelegate cannot be null");
		if (updateDelegate == null) throw new UnassignedReferenceException("updateDelegate cannot be null");
		this.startDelegate = startDelegate;
		this.updateDelegate = updateDelegate;
	}
	public AnimationStep(AnimationDelegate startDelegate, AnimationDelegate updateDelegate, SkipAnimationDelegate skipDelegate)
	{
		if (startDelegate == null) throw new UnassignedReferenceException("startDelegate cannot be null");
		if (updateDelegate == null) throw new UnassignedReferenceException("updateDelegate cannot be null");
		if (skipDelegate == null) throw new UnassignedReferenceException("skipDelegate cannot be null");
		this.startDelegate = startDelegate;
		this.updateDelegate = updateDelegate;
		this.skipDelegate = skipDelegate;
		skipOnTap = true;
	}
	
	public AnimationDelegate startDelegate;
	public AnimationDelegate updateDelegate;
	public SkipAnimationDelegate skipDelegate;
	public readonly bool skipOnTap = false;
}


public class AnimationQueue
{
	public delegate void AnimationQueueComplete(AnimationQueue animationQueue);
	public event AnimationQueueComplete onAnimationQueueComplete;
	
	private Queue<AnimationStep> queue;
	private AnimationStep currentAnimationStep;
	
	public AnimationQueue()
	{
		queue = new Queue<AnimationStep>(8);
		
		currentAnimationStep = null;
	}
	
	public void addAnimationStep(AnimationStep step)
	{
		queue.Enqueue(step);
	}
	
	public bool beginNextAnimation()
	{
		currentAnimationStep = null;
		if (queue.Count > 0)
			currentAnimationStep = queue.Dequeue();
		
		if (currentAnimationStep != null)
		{
			currentAnimationStep.startDelegate();
			return true;
		}
		
		if (onAnimationQueueComplete != null)
			onAnimationQueueComplete(this);
		
		return false;
	}
	
	public void updateAnimation()
	{
		if (currentAnimationStep != null)
		{
			if (currentAnimationStep.skipOnTap && Input.GetMouseButtonDown(0))
			{
				switch (currentAnimationStep.skipDelegate())
				{
				case AnimationStep.SkipResult.SkipOne:
					beginNextAnimation();
					return;
				case AnimationStep.SkipResult.SkipAll:
					stopAndClear();
					return;
				}
			}
			if (currentAnimationStep.updateDelegate != null)
				currentAnimationStep.updateDelegate();
		}
	}
	
	public void stopAndClear()
	{
		currentAnimationStep = null;
		queue.Clear();
		if (onAnimationQueueComplete != null)
			onAnimationQueueComplete(this);
	}
}