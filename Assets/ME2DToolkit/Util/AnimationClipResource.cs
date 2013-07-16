using System;
using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// Animation clip resource.
/// </summary>
[Serializable]
public class AnimationClipResource
{
	[XmlAttribute("name")]
	public string name = "New Clip";
	public Material atlas;
	public TextAsset textureAtlasFrames;
}