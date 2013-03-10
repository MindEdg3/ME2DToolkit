using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MESprite
{
	#region Fields
	[HideInInspector]
	[SerializeField]
	public AnimationSequence _framesSequence;
	[HideInInspector]
	[SerializeField]
	public float _speed = 1.0f;
	[HideInInspector]
	[SerializeField]
	public float lastTimeFrameChanged;
	[HideInInspector]
	[SerializeField]
	public int frameIndex;
	#endregion
	
	#region Properties
	/// <summary>
	/// Gets a value indicating whether this animation ended (animation at last frame).
	/// </summary>
	/// <value>
	/// <c>true</c> if this animation ended; otherwise, <c>false</c>.
	/// </value>
	public bool IsAnimationEnded {
		get {
			return frameIndex == FramesSequence.frames.Count - 1;
		}
	}
	
	/// <summary>
	/// Frames sequence of animation.
	/// </summary>
	/// <value>
	/// Frames sequence of animation.
	/// </value>
	public AnimationSequence FramesSequence {
		get {
			return _framesSequence;
		}
		set {
			if (_framesSequence != value) {
				_framesSequence = value;
				isNeedToRefresh = true;
			}
		}
	}
	
	/// <summary>
	/// The speed of animation.
	/// </summary>
	/// <value>
	/// The speed of animation.
	/// </value>
	public float Speed {
		get {
			return _speed;
		}
		set {
			if (_speed != value) {
				_speed = value;
				isNeedToRefresh = true;
			}
		}
	}
	#endregion
	
	public virtual void Update ()
	{
		if (Application.isPlaying) {
			if ((lastTimeFrameChanged + 1 / Speed / FramesSequence.speed / FramesSequence.frames [frameIndex].speed) < Time.time) {
				// switch to next frame.
				frameIndex++;
				if (frameIndex >= FramesSequence.frames.Count) {
					frameIndex = 0;
				}
				lastTimeFrameChanged = Time.time;
				RefreshAnimatedFrame ();
			
				isNeedToRefresh = true;
			}
		}
	}

	public void RefreshAnimatedFrame ()
	{
		AnimFrame currentFrame = FramesSequence.frames [frameIndex];
		if (!currentFrame.framesMap.Equals (MyFramesMap)) {
			MyFramesMap = currentFrame.framesMap;
		}
		FrameName = currentFrame.frameName;
	}
}