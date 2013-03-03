using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Sprite editor GUI for inspector.
/// </summary>
[CustomEditor(typeof(MESprite))]
[CanEditMultipleObjects]
public class MESpriteEditor : Editor
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
	/// Gets or sets the frame boundaries.
	/// </summary>
	/// <value>
	/// The frame boundaries.
	/// </value>
	private SpriteBounds FrameBoundaries {
		get {
			if (_frameBoundaries == null) {
				_frameBoundaries = MySprite.MyFramesMap.spriteBounds.Find (sb => sb.name == MySprite.FrameName);
			}
			return _frameBoundaries;
		}
		set {
			_frameBoundaries = value;
		}
	}
	
	/// <summary>
	/// Creates custom inspector elements.
	/// </summary>/
	public override void OnInspectorGUI ()
	{
		MySprite.HorizontalSpriteAlignment = (SpriteHorizontalAlignment)EditorGUILayout.EnumPopup ("Horizontal Alignment", MySprite.HorizontalSpriteAlignment);
		MySprite.VerticalSpriteAlignment = (SpriteVerticalAlignment)EditorGUILayout.EnumPopup ("Vertical Alignment", MySprite.VerticalSpriteAlignment);
		
		GameObject selectedMapGO = EditorGUILayout.ObjectField ("Frame Map", MySprite.MyFramesMap, typeof(GameObject), false) as GameObject;
		if (selectedMapGO != null) {
			FramesMap _fm = selectedMapGO.GetComponent<FramesMap> ();
			if (_fm != null) {
				MySprite.MyFramesMap = _fm; 
			}
		}
		
		if (MySprite.MyFramesMap != null) {
			string[] frameNames = new string[MySprite.MyFramesMap.spriteBounds.Count];
			for (int i = 0; i< frameNames.Length; i++) {
				frameNames [i] = MySprite.MyFramesMap.spriteBounds [i].name;
				if (frameNames [i].Equals (MySprite.FrameName)) {
					selectedFrameIndex = i;
				}
			}
			selectedFrameIndex = EditorGUILayout.Popup ("Frame Name", selectedFrameIndex, frameNames);
			if (!MySprite.FrameName.Equals (MySprite.MyFramesMap.spriteBounds [selectedFrameIndex].name)) {
				MySprite.FrameName = MySprite.MyFramesMap.spriteBounds [selectedFrameIndex].name;
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
				MySprite.MyFramesMap.atlas.mainTexture,
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
				MySprite.FrameName + " " + MySprite.MyFramesMap.atlas.mainTexture.width * FrameBoundaries.textureScale.x + "x" + MySprite.MyFramesMap.atlas.mainTexture.height * FrameBoundaries.textureScale.y
			);
		}
		MySprite.RefreshSprite ();
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
