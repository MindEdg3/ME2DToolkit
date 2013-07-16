using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SpriteAtlasMaker : EditorWindow
{
	Texture2D atlasTexture = null;
	TextAsset textureAtlasData = null;
	string newAtlasName = "New SpriteAtlas" + Random.Range (100, 1000);
	string atlasPath = "Assets/";

	[MenuItem("Window/MEAnimation/Create Sprite Atlas")]
	static void OpenWindow ()
	{
		EditorWindow.GetWindow<SpriteAtlasMaker> ("New Sprite Atlas");
	}
	
	void Awake ()
	{
		for (int i = 0; i < Selection.objects.Length; i++) {
			if (Selection.objects [i].GetType () == typeof(Texture2D)) {
				atlasTexture = Selection.objects [i] as Texture2D;
				break;
			}
		}
		for (int i = 0; i < Selection.objects.Length; i++) {
			if (Selection.objects [i].GetType () == typeof(TextAsset)) {
				textureAtlasData = Selection.objects [i] as TextAsset;
				break;
			}
		}
	}
	
	void OnGUI ()
	{	
		Color nativeColor = GUI.color;
		
		newAtlasName = EditorGUILayout.TextField ("Atlas Name", newAtlasName);
		atlasTexture = EditorGUILayout.ObjectField ("Atlas Texture", atlasTexture, typeof(Texture2D), false) as Texture2D;
		textureAtlasData = EditorGUILayout.ObjectField ("Texture Atlas Data", textureAtlasData, typeof(TextAsset), false) as TextAsset;
		
		
		if (string.IsNullOrEmpty (newAtlasName) || atlasTexture == null || textureAtlasData == null) {
			GUI.color = Color.red;
			GUILayout.Button ("Create");
		} else {
			GUI.color = Color.green;
			if (GUILayout.Button ("Create")) {
				atlasPath = Path.GetDirectoryName (AssetDatabase.GetAssetPath (textureAtlasData));
				CreateNewAtlas (atlasPath, newAtlasName, atlasTexture, textureAtlasData);
			}
		}
		GUI.color = nativeColor;
	}
	
	private void CreateNewAtlas (string atlasPath, string atlasName, Texture2D textureAtlas, TextAsset textureData)
	{
		// Try to load the material
		string materialPath = atlasPath + "/" + atlasName + ".mat";
		Material spriteAtlasMaterial = AssetDatabase.LoadAssetAtPath (materialPath, typeof(Material)) as Material;

		// If the material with such name doesn't exist, create it
		if (spriteAtlasMaterial == null) {
			Shader shader = Shader.Find ("Unlit/Transparent");
			spriteAtlasMaterial = new Material (shader);

			// Save the material
			AssetDatabase.CreateAsset (spriteAtlasMaterial, materialPath);
			AssetDatabase.Refresh ();

			// Load the material so it's usable
			spriteAtlasMaterial = AssetDatabase.LoadAssetAtPath (materialPath, typeof(Material)) as Material;
		}
		spriteAtlasMaterial.mainTexture = textureAtlas;
		
		// Create new SpriteAtlas prefab
		Object newPrefab = PrefabUtility.CreateEmptyPrefab (atlasPath + "/" + atlasName + ".prefab");
		GameObject newSpriteAtlasGO = new GameObject ("atlas_" + name, typeof(SpriteAtlas));
		PrefabUtility.ReplacePrefab (newSpriteAtlasGO, newPrefab);
		DestroyImmediate (newSpriteAtlasGO);
		AssetDatabase.Refresh ();
		
		// Load and set values to new atlas
		newSpriteAtlasGO = AssetDatabase.LoadAssetAtPath (atlasPath + "/" + atlasName + ".prefab", typeof(GameObject)) as GameObject;
		SpriteAtlas newSpriteAtlas = newSpriteAtlasGO.GetComponent<SpriteAtlas> ();
		newSpriteAtlas.atlas = spriteAtlasMaterial;
		newSpriteAtlas.spriteBounds = ReadXML (textureData);
		
		// Create AnimationSequence automatically
		string animationFolder = AssetDatabase.GUIDToAssetPath (AssetDatabase.CreateFolder (atlasPath, "/animation_" + atlasName));

		// Create new AnimationSequence prefab
		Object newAnimationSequencePrefab = PrefabUtility.CreateEmptyPrefab (animationFolder + "/animation_" + atlasName + ".prefab");
		GameObject newAnimationSequenceGO = new GameObject ("animation_" + name, typeof(AnimationSequence));
		PrefabUtility.ReplacePrefab (newAnimationSequenceGO, newAnimationSequencePrefab);
		DestroyImmediate (newAnimationSequenceGO);
		AssetDatabase.Refresh ();
		
		// Load and set values to new atlas
		newAnimationSequenceGO = AssetDatabase.LoadAssetAtPath (animationFolder + "/animation_" + atlasName + ".prefab", typeof(GameObject)) as GameObject;
		AnimationSequence newAnimationSequence = newAnimationSequenceGO.GetComponent<AnimationSequence> ();
		for (int i = 0; i < newSpriteAtlas.spriteBounds.Count; i++) {
			newAnimationSequence.sprites.Add (new AnimSprite (newSpriteAtlas, newSpriteAtlas.spriteBounds [i].name, 1f));
		}
		
		// Set selection in inspector
		Selection.objects = new Object[] {newSpriteAtlasGO};
	}
	
	/*
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
	*/
	
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
