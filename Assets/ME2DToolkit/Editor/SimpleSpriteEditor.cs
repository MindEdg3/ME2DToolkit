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
	protected int selectedSpriteIndex;
	protected SpriteBounds _spriteBoundaries;
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
	/// Gets or sets the sprite boundaries.
	/// </summary>
	/// <value>
	/// The sprite boundaries.
	/// </value>
	protected SpriteBounds SpriteBoundaries {
		get {
			if (_spriteBoundaries == null) {
				_spriteBoundaries = MySpritesAtlas.spriteBounds.Find (sb => sb.name == SpriteName);
				if (_spriteBoundaries == null) {
					_spriteBoundaries = MySpritesAtlas.spriteBounds [0];
				}	
			}
			return _spriteBoundaries;
		}
		set {
			_spriteBoundaries = value;
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
	
	protected SpriteAtlas MySpritesAtlas {
		get {
			return MySprite.MySpritesAtlas;
		}
		set {
			if (MySprite.MySpritesAtlas != value) {
				Undo.RegisterUndo (MySprite, "Sprite atlas change");
				
				MySprite.MySpritesAtlas = value;
				if (MySprite.MySpritesAtlas == null) {
					SpriteBoundaries = null;
				} else {
					_spriteBoundaries = MySpritesAtlas.spriteBounds.Find (sb => sb.name == SpriteName);
				}
				isNeedToRefresh = true;
			}
		}
	}
	
	protected string SpriteName {
		get {
			return MySprite.SpriteName;
		}
		set {
			if (!MySprite.SpriteName.Equals (value)) {
				Undo.RegisterUndo (MySprite, "Sprite name change");
				
				MySprite.SpriteName = value;
				_spriteBoundaries = MySpritesAtlas.spriteBounds.Find (sb => sb.name == SpriteName);
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
		DrawSpriteAtlas ();
		
		if (MySpritesAtlas != null) {
			DrawSpriteName ();
			DrawScale ();
			DrawAlignment ();
			DrawBakeScaleBtn ();
			DrawSpritePreview ();
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
	
	protected virtual void DrawSpriteAtlas ()
	{
		MySpritesAtlas = EditorGUILayout.ObjectField ("Sprite Atlas", MySpritesAtlas, typeof(SpriteAtlas), false) as SpriteAtlas;
	}

	protected virtual void DrawSpriteName ()
	{
		string[] spriteNames = new string[MySpritesAtlas.spriteBounds.Count];
		for (int i = 0; i< spriteNames.Length; i++) {
			spriteNames [i] = MySpritesAtlas.spriteBounds [i].name;
			if (spriteNames [i].Equals (SpriteName)) {
				selectedSpriteIndex = i;
			}
		}
		selectedSpriteIndex = EditorGUILayout.Popup ("Sprite Name", selectedSpriteIndex, spriteNames);
		SpriteName = MySpritesAtlas.spriteBounds [selectedSpriteIndex].name;
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

	protected virtual void DrawSpritePreview ()
	{
		Rect rect = GUILayoutUtility.GetLastRect ();
		if (SpriteBoundaries.textureTiling.x != 0) {
			GUILayout.Space (rect.yMin + 30f + (Screen.width - 32f) * SpriteBoundaries.textureTiling.y / SpriteBoundaries.textureTiling.x);
		
			GUI.DrawTextureWithTexCoords (
				new Rect (
					16f,
					rect.yMin + 24f,
					Screen.width - 32f,
					(Screen.width - 32f) * SpriteBoundaries.textureTiling.y / SpriteBoundaries.textureTiling.x
				),
				MySpritesAtlas.atlas.mainTexture,
				new Rect (
					SpriteBoundaries.textureOffset.x,
					SpriteBoundaries.textureOffset.y,
					SpriteBoundaries.textureTiling.x,
					SpriteBoundaries.textureTiling.y
				)
			);
			EditorGUI.DropShadowLabel (
				new Rect (
						0f,
						rect.yMin + 30f + (Screen.width - 32f) * SpriteBoundaries.textureTiling.y / SpriteBoundaries.textureTiling.x,
						Screen.width,
						24f
				),
				SpriteName + "\n" + 
				MySpritesAtlas.atlas.mainTexture.width * SpriteBoundaries.textureTiling.x + "x" + 
				MySpritesAtlas.atlas.mainTexture.height * SpriteBoundaries.textureTiling.y
			);
		}
	}
}
