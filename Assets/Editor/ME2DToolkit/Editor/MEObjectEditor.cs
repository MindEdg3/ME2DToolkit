using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MESprite))]
public class MEObjectEditor : Editor
{
	private int selectedFrameIndex;
	private SpriteBounds _frameBoundaries;
	MESprite _mySprite;
	
	/// <summary>
	/// Link to target object
	/// </summary>
	/// <value>
	/// My sprite.
	/// </value>
	private MESprite MySprite {
		get {
			if (_mySprite == null) {
				_mySprite = target as MESprite;
			}
			return _mySprite;
		}
	}

	/// <summary>
	/// Gets or sets the name of the frame.
	/// </summary>
	/// <value>
	/// The name of the frame.
	/// </value>
	private string FrameName {
		get {
			return MySprite.frameName;
		}
		set {
			MySprite.frameName = value;
			FrameBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == FrameName);
			string[] frameNames = new string[MyFramesMap.spriteBounds.Count];
			for (int i = 0; i< frameNames.Length; i++) {
				frameNames [i] = MyFramesMap.spriteBounds [i].name;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the frame boundaries.
	/// </summary>
	/// <value>
	/// The frame boundaries.
	/// </value>
	private SpriteBounds FrameBoundaries {
		get {
			if (_frameBoundaries == null) {
				_frameBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == FrameName);
			}
			return _frameBoundaries;
		}
		set {
			_frameBoundaries = value;
		}
	}
	
	/// <summary>
	/// Gets or sets my frames map.
	/// </summary>
	/// <value>
	/// My frames map.
	/// </value>
	private FramesMap MyFramesMap {
		get {
			return MySprite.framesMap;
		}
		set {
			MySprite.framesMap = value;
		}	
	}
	
	/// <summary>
	/// Creates custom inspector elements.
	/// </summary>/
	public override void OnInspectorGUI ()
	{
		GameObject selectedMapGO = EditorGUILayout.ObjectField ("Frame Map", MyFramesMap, typeof(GameObject), false) as GameObject;
		if (selectedMapGO != null) {
			MyFramesMap = selectedMapGO.GetComponent<FramesMap> ();
		}
		
		if (MyFramesMap != null) {
			string[] frameNames = new string[MyFramesMap.spriteBounds.Count];
			for (int i = 0; i< frameNames.Length; i++) {
				frameNames [i] = MyFramesMap.spriteBounds [i].name;
				if (frameNames [i].Equals (FrameName)) {
					selectedFrameIndex = i;
				}
			}
			selectedFrameIndex = EditorGUILayout.Popup ("Frame Name", selectedFrameIndex, frameNames);
			if (!FrameName.Equals (MyFramesMap.spriteBounds [selectedFrameIndex].name)) {
				FrameName = MyFramesMap.spriteBounds [selectedFrameIndex].name;
			}
			GUILayout.Button ("Bake Scale");
			
			// Image preview
			Rect rect = GUILayoutUtility.GetLastRect ();
			GUILayout.Space (rect.yMin + 30f + (Screen.width - 32f) * FrameBoundaries.textureScale.y / FrameBoundaries.textureScale.x);
			
			GUI.DrawTextureWithTexCoords (
				new Rect (
					16f,
					rect.yMin + 24f,
					Screen.width - 32f,
					(Screen.width - 32f) * FrameBoundaries.textureScale.y / FrameBoundaries.textureScale.x
				),
				MyFramesMap.atlas.mainTexture,
				new Rect (
					FrameBoundaries.textureOffset.x,
					FrameBoundaries.textureOffset.y,
					FrameBoundaries.textureScale.x,
					FrameBoundaries.textureScale.y
				)
			);
			EditorGUI.DropShadowLabel (new Rect (
					0f,
					rect.yMin + 30f + (Screen.width - 32f) * FrameBoundaries.textureScale.y / FrameBoundaries.textureScale.x,
					Screen.width,
					12f
				),
				FrameName + " " + MyFramesMap.atlas.mainTexture.width * FrameBoundaries.textureScale.x + "x" + MyFramesMap.atlas.mainTexture.height * FrameBoundaries.textureScale.y
			);
		}
	}
	
	#region Static Context
	static Texture2D mWhiteTex;
	
	/// <summary>
	/// Create a white dummy texture.
	/// </summary>

	static Texture2D CreateDummyTex ()
	{
		Texture2D tex = new Texture2D (1, 1);
		tex.name = "[Generated] Dummy Texture";
		tex.hideFlags = HideFlags.DontSave;
		tex.filterMode = FilterMode.Point;
		tex.SetPixel (0, 0, Color.white);
		tex.Apply ();
		return tex;
	}
	
	/// <summary>
	/// Returns a blank usable 1x1 white texture.
	/// </summary>

	static public Texture2D blankTexture {
		get {
			if (mWhiteTex == null)
				mWhiteTex = CreateDummyTex ();
			return mWhiteTex;
		}
	}
	#endregion
}
