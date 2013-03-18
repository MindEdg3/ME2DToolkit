using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Sprite editor GUI for inspector.
/// </summary>
[CustomEditor(typeof(MESprite))]
[CanEditMultipleObjects]
public class SimpleSpriteEditor : Editor
{
	protected bool isNeedToRefresh = false;
	protected int selectedFrameIndex;
	protected SpriteBounds _frameBoundaries;
	protected MESprite _mySprite;
	
	#region Properties
	
	/// <summary>
	/// Link to target object
	/// </summary>
	/// <value>
	/// My sprite.
	/// </value>
	protected MESprite MySprite {
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
	protected SpriteBounds FrameBoundaries {
		get {
			if (_frameBoundaries == null) {
				_frameBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == FrameName);
				if (_frameBoundaries == null) {
					_frameBoundaries = MyFramesMap.spriteBounds [0];
				}	
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
		DrawFramesMap ();
		
		if (MyFramesMap != null) {
			DrawFrameName ();
			DrawScale ();
			DrawAlignment ();
			DrawBakeScaleBtn ();
			DrawImagePreview ();
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
	
	protected virtual void DrawFramesMap ()
	{
		MyFramesMap = EditorGUILayout.ObjectField ("Frame Map", MyFramesMap, typeof(FramesMap), false) as FramesMap;
	}

	protected virtual void DrawFrameName ()
	{
		string[] frameNames = new string[MyFramesMap.spriteBounds.Count];
		for (int i = 0; i< frameNames.Length; i++) {
			frameNames [i] = MyFramesMap.spriteBounds [i].name;
			if (frameNames [i].Equals (FrameName)) {
				selectedFrameIndex = i;
			}
		}
		selectedFrameIndex = EditorGUILayout.Popup ("Frame Name", selectedFrameIndex, frameNames);
		FrameName = MyFramesMap.spriteBounds [selectedFrameIndex].name;
	}
	
	protected virtual void DrawScale ()
	{
		Scale = EditorGUILayout.FloatField ("Scale", Scale);
	}
	
	protected virtual void DrawAlignment ()
	{
		HorizontalSpriteAlignment = (SpriteHorizontalAlignment)EditorGUILayout.EnumPopup ("Horizontal Alignment", HorizontalSpriteAlignment);
		VerticalSpriteAlignment = (SpriteVerticalAlignment)EditorGUILayout.EnumPopup ("Vertical Alignment", VerticalSpriteAlignment);
	}
	
	protected virtual void DrawBakeScaleBtn ()
	{
		if (GUILayout.Button ("Bake Scale")) {
			MESpritesManager.BakeScale (MySprite);
		}
	}

	protected virtual void DrawImagePreview ()
	{
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
		EditorGUI.DropShadowLabel (
			new Rect (
					0f,
					rect.yMin + 30f + (Screen.width - 32f) * FrameBoundaries.textureScale.y / FrameBoundaries.textureScale.x,
					Screen.width,
					24f
			),
			FrameName + "\n" + 
			MyFramesMap.atlas.mainTexture.width * FrameBoundaries.textureScale.x + "x" + 
			MyFramesMap.atlas.mainTexture.height * FrameBoundaries.textureScale.y
		);
	}
}
