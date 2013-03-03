using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSequence : MonoBehaviour
{
	public float speed = 1f;
	public List<AnimFrame> frames = new List<AnimFrame> ();
}

[Serializable]
public class AnimFrame
{	
	public string frameName;
	public float speed = 1f;
	
	public AnimFrame (string frameName, float speed)
	{
		this.frameName = frameName;
		this.speed = speed;
	}
}