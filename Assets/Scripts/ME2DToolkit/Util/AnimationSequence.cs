using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSequence : MonoBehaviour
{
	public float speed = 1f;
	public List<AnimSprite> sprites = new List<AnimSprite> ();
}

[Serializable]
public class AnimSprite
{	
	public SpriteAtlas spritesAtlas;
	public string spriteName;
	public float speed = 1f;
	
	public AnimSprite (SpriteAtlas spritesAtlas, string spriteName, float speed)
	{
		this.spritesAtlas = spritesAtlas;
		this.spriteName = spriteName;
		this.speed = speed;
	}
}