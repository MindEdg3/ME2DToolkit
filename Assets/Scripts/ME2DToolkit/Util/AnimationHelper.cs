using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
	public List<AnimationClipResource> clipResources;
	
	#region Singleton
	private static AnimationHelper instance;

	public static AnimationHelper Instance {
		get {
			if (instance == null) {
				instance = GameObject.Find ("Animation").GetComponent<AnimationHelper> ();
			}
			return instance;
		}
	}
	#endregion
	
	void Awake ()
	{
		instance = this;
		LoadAnimations ();
	}

	void LoadAnimations ()
	{
		for (int i = 0; i < clipResources.Count; i++) {
			FramesMap newFramesMap = new GameObject ("frames_" + clipResources [i].name, typeof(FramesMap)).GetComponent<FramesMap> ();
			newFramesMap.clipName = clipResources [i].name;
			newFramesMap.atlas = clipResources [i].atlas;
			newFramesMap.spriteBounds = ReadXML (clipResources [i].textureAtlasFrames);
			
			AnimationSequence newSequence = new GameObject ("sequence_" + clipResources [i].name, typeof(AnimationSequence)).GetComponent<AnimationSequence> ();
			//newSequence.framesMap = newFramesMap;
			for (int j = 0; j < newFramesMap.spriteBounds.Count; j++) {
				newSequence.frames.Add (new AnimFrame (newFramesMap.spriteBounds [j].name, 1f));
			}
		}
	}
	
	public static List<SpriteBounds> ReadXML (TextAsset xmlSource)
	{
		List<SpriteBounds> spritesBounds = new List<SpriteBounds> ();
		XmlTextReader reader = null;
		
		try {
			reader = new XmlTextReader (new StringReader (xmlSource.text));
			reader.WhitespaceHandling = WhitespaceHandling.None;
			
			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "TextureAtlas") {
					Vector2 atlasSize = new Vector2 (
						int.Parse (reader.GetAttribute ("width")),
						int.Parse (reader.GetAttribute ("height")));
					
					while (reader.Read() && reader.Name == "sprite") {
						float textureXScale = float.Parse (reader.GetAttribute ("w")) / atlasSize.x;
						float textureYScale = float.Parse (reader.GetAttribute ("h")) / atlasSize.y;
						float textureXOffset = float.Parse (reader.GetAttribute ("x")) / atlasSize.x;
						float textureYOffset = (1 - float.Parse (reader.GetAttribute ("y")) - float.Parse (reader.GetAttribute ("h"))) / atlasSize.y;
						
						SpriteBounds newBounds = new SpriteBounds (
							reader.GetAttribute ("n"),
							new Vector2 (textureXOffset, textureYOffset),
							new Vector2 (textureXScale, textureYScale),
							atlasSize.x / 1024
							);
						
						spritesBounds.Add (newBounds);
					}
				}
			}
		} finally {
			if (reader != null) {
				reader.Close ();
			}
		}
		
		return spritesBounds;
	}
}

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

/// <summary>
/// Sprite bounds, that primarilly used to keep information about scale and offset of texture. Also keeps it's name and size ratio.
/// </summary>
[Serializable]
public class SpriteBounds
{
	public string name;
	public Vector2 textureOffset = Vector2.zero;
	public Vector2 textureScale = Vector2.zero;
	public float spriteSizeRatio = 1f;
	
	private SpriteBounds ()
	{
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="SpriteBounds"/> class.
	/// </summary>
	/// <param name='name'>
	/// Sprite name.
	/// </param>
	/// <param name='offset'>
	/// Sprite offset.
	/// </param>
	/// <param name='scale'>
	/// Sprite scale.
	/// </param>
	/// <param name='spriteSizeRatio'>
	/// Sprite size ratio from 1024x1024 pixels.
	/// </param>
	public SpriteBounds (string name, Vector2 offset, Vector2 scale, float spriteSizeRatio)
	{
		this.name = name;
		this.textureOffset = offset;
		this.textureScale = scale;
		this.spriteSizeRatio = spriteSizeRatio;
	}
	
	/// <summary>
	/// Serves as a hash function for a <see cref="SpriteBounds"/> object.
	/// </summary>
	/// <returns>
	/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.
	/// </returns>
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
	
	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="SpriteBounds"/>.
	/// </summary>
	/// <param name='other'>
	/// The <see cref="System.Object"/> to compare with the current <see cref="SpriteBounds"/>.
	/// </param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="SpriteBounds"/>;
	/// otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals (object other)
	{
		SpriteBounds otherSB;
		
		otherSB = other as SpriteBounds;
		
		if (otherSB == null) {
			return false;
		} else {
			return Equals (otherSB);
		}
	}
	
	/// <summary>
	/// Determines whether the specified <see cref="SpriteBounds"/> is equal to the current <see cref="SpriteBounds"/>.
	/// </summary>
	/// <param name='otherSB'>
	/// The <see cref="SpriteBounds"/> to compare with the current <see cref="SpriteBounds"/>.
	/// </param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="SpriteBounds"/> is equal to the current <see cref="SpriteBounds"/>;
	/// otherwise, <c>false</c>.
	/// </returns>
	public bool Equals (SpriteBounds otherSB)
	{
		return (otherSB.name == this.name) && (this.textureOffset.Equals (otherSB.textureOffset)) && (textureScale.Equals (otherSB.textureScale)) && (spriteSizeRatio == spriteSizeRatio);
	}
}