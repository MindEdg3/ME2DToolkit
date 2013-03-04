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
	protected bool isNeedToRefresh = false;
	private int selectedFrameIndex;
	private SpriteBounds _frameBoundaries;
	private MESprite _mySprite;
	
	#region Properties
	
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
				_frameBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == FrameName);
			}
			return _frameBoundaries;
		}
		set {
			_frameBoundaries = value;
		}
	}
	
	protected SpriteHorizontalAlignment HorizontalSpriteAlignment {
		get {
			return MySprite.HorizontalSpriteAlignment;
		}
		set {
			if (MySprite.HorizontalSpriteAlignment != value) {
				Undo.RegisterUndo (MySprite, "Sprite horizontal alignment change");
				
				MySprite.HorizontalSpriteAlignment = value;
				isNeedToRefresh = true;
			}
		}
	}
	
	protected SpriteVerticalAlignment VerticalSpriteAlignment {
		get {
			return MySprite.VerticalSpriteAlignment;
		}
		set {
			if (MySprite.VerticalSpriteAlignment != value) {
				Undo.RegisterUndo (MySprite, "Sprite vertical alignment change");
				
				MySprite.VerticalSpriteAlignment = value;
				isNeedToRefresh = true;
			}
		}
	}
	
	protected FramesMap MyFramesMap {
		get {
			return MySprite.MyFramesMap;
		}
		set {
			if (MySprite.MyFramesMap != value) {
				Undo.RegisterUndo (MySprite, "Sprite frame map change");
				
				MySprite.MyFramesMap = value;
				if (MySprite.MyFramesMap == null) {
					FrameBoundaries = null;
				} else {
					_frameBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == FrameName);
				}
				isNeedToRefresh = true;
			}
		}
	}
	
	protected string FrameName {
		get {
			return MySprite.FrameName;
		}
		set {
			if (!MySprite.FrameName.Equals (value)) {
				Undo.RegisterUndo (MySprite, "Sprite frame name change");
				
				MySprite.FrameName = value;
				_frameBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == FrameName);
				isNeedToRefresh = true;
			}
		}
	}
	
	protected float Scale {
		get {
			return MySprite.Scale;
		}
		set {
			if (MySprite.Scale != value) {
				Undo.RegisterUndo (MySprite, "Sprite scale change");
				
				MySprite.Scale = value;
				isNeedToRefresh = true;
			}
		}
	}
	#endregion
	
	/// <summary>
	/// Creates custom inspector elements.
	/// </summary>/
	public override void OnInspectorGUI ()
	{
		MyFramesMap = EditorGUILayout.ObjectField ("Frame Map", MyFramesMap, typeof(FramesMap), false) as FramesMap;
		/*
		GameObject selectedMapGO = EditorGUILayout.ObjectField ("Frame Map", MyFramesMap, typeof(GameObject), false) as GameObject;
		if (selectedMapGO != null) {
			FramesMap _fm = selectedMapGO.GetComponent<FramesMap> ();
			MyFramesMap = _fm;
		}*/
		
		if (MyFramesMap != null) {
			string[] frameNames = new string[MyFramesMap.spriteBounds.Count];
			for (int i = 0; i< frameNames.Length; i++) {
				frameNames [i] = MyFramesMap.spriteBounds [i].name;
				if (frameNames [i].Equals (FrameName)) {
					selectedFrameIndex = i;
				}
			}
			selectedFrameIndex = EditorGUILayout.Popup ("Frame Name", selectedFrameIndex, frameNames);
			FrameName = MyFramesMap.spriteBounds [selectedFrameIndex].name;
			
			Scale = EditorGUILayout.FloatField ("Scale", Scale);
			
			HorizontalSpriteAlignment = (SpriteHorizontalAlignment)EditorGUILayout.EnumPopup ("Horizontal Alignment", HorizontalSpriteAlignment);
			VerticalSpriteAlignment = (SpriteVerticalAlignment)EditorGUILayout.EnumPopup ("Vertical Alignment", VerticalSpriteAlignment);
			
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
					24f
				),
				FrameName + "\n" + 
				MyFramesMap.atlas.mainTexture.width * FrameBoundaries.textureScale.x + "x" + MyFramesMap.atlas.mainTexture.height * FrameBoundaries.textureScale.y
			);
		}
		
		
		if (Event.current.type == EventType.ValidateCommand) {
			switch (Event.current.commandName) {
			case "UndoRedoPerformed":
				MySprite.RefreshSprite ();
				isNeedToRefresh = false;
				break;
			}
		}
		
		if (isNeedToRefresh) {
			MySprite.RefreshSprite ();
			isNeedToRefresh = false;
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
		tex.name = "Dummy Texture";
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
