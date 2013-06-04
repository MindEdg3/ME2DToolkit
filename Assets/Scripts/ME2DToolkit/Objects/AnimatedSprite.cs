using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MESprite
{
	#region Fields
	[HideInInspector]
	[SerializeField]
	public AnimationSequence _spritesSequence;
	[HideInInspector]
	[SerializeField]
	public float _speed = 1.0f;
	[HideInInspector]
	[SerializeField]
	public float lastTimeSpriteChanged;
	[HideInInspector]
	[SerializeField]
	public int spriteIndex;
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
			return spriteIndex == SpritesSequence.sprites.Count - 1;
		}
	}
	
	/// <summary>
	/// Frames sequence of animation.
	/// </summary>
	/// <value>
	/// Frames sequence of animation.
	/// </value>
	public AnimationSequence SpritesSequence {
		get {
			return _spritesSequence;
		}
		set {
			if (_spritesSequence != value) {
				_spritesSequence = value;
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
			if ((lastTimeSpriteChanged + 1 / Speed / SpritesSequence.speed / SpritesSequence.sprites [spriteIndex].speed) < Time.time) {
				// switch to next frame.
				spriteIndex++;
				if (spriteIndex >= SpritesSequence.sprites.Count) {
					spriteIndex = 0;
				}
				lastTimeSpriteChanged = Time.time;
				RefreshAnimatedSprite ();
			
				isNeedToRefresh = true;
			}
		}
	}

	public void RefreshAnimatedSprite ()
	{
		AnimSprite currentSprite = SpritesSequence.sprites [spriteIndex];
		if (!currentSprite.spritesAtlas.Equals (MySpritesAtlas)) {
			MySpritesAtlas = currentSprite.spritesAtlas;
		}
		SpriteName = currentSprite.spriteName;
	}
}