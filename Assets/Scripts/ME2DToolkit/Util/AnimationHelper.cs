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
			SpriteAtlas newFramesMap = new GameObject ("frames_" + clipResources [i].name, typeof(SpriteAtlas)).GetComponent<SpriteAtlas> ();
//			newFramesMap.clipName = clipResources [i].name;
			newFramesMap.atlas = clipResources [i].atlas;
			newFramesMap.spriteBounds = ReadXML (clipResources [i].textureAtlasFrames);
			
			AnimationSequence newSequence = new GameObject ("sequence_" + clipResources [i].name, typeof(AnimationSequence)).GetComponent<AnimationSequence> ();
			//newSequence.framesMap = newFramesMap;
			for (int j = 0; j < newFramesMap.spriteBounds.Count; j++) {
				newSequence.sprites.Add (new AnimSprite (newFramesMap, newFramesMap.spriteBounds [j].name, 1f));
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
						int.Parse (reader.GetAttribute ("height"))
					);
					
					while (reader.Read() && reader.Name == "sprite") {
						float textureXOffset = float.Parse (reader.GetAttribute ("x")) / atlasSize.x;
						float textureYOffset = (-float.Parse (reader.GetAttribute ("y")) - float.Parse (reader.GetAttribute ("h"))) / atlasSize.y;
						float textureXScale = float.Parse (reader.GetAttribute ("w")) / atlasSize.x;
						float textureYScale = float.Parse (reader.GetAttribute ("h")) / atlasSize.y;
						
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
