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
	public FramesMap framesMap;
	public string frameName;
	public float speed = 1f;
	
	public AnimFrame (FramesMap framesMap, string frameName, float speed)
	{
		this.framesMap = framesMap;
		this.frameName = frameName;
		this.speed = speed;
	}
}