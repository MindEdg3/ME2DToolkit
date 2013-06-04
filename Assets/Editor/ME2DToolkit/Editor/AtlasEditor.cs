using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SpriteAtlas))]
[CanEditMultipleObjects]
public class AtlasEditor : Editor
{
	private int _selectedSprite;
	private bool isPreviewSettingsExpanded;
	protected SpriteAtlas _mySpriteAtlas;
	
	#region properties
	private string CurrentSpriteName {
		set {
			SpriteBounds currentSprite = MySpriteAtlas.spriteBounds [_selectedSprite];
			if (currentSprite.name != value) {
				currentSprite.name = value;
			}
		}
		get {
			if (MySpriteAtlas.spriteBounds.Count > _selectedSprite && MySpriteAtlas.spriteBounds [_selectedSprite] != null) {
				return MySpriteAtlas.spriteBounds [_selectedSprite].name;
			} else {
				return null;
			}
		}
	}

	private Vector2 CurrentSpriteTextureOffset {
		set {
			SpriteBounds currentSprite = MySpriteAtlas.spriteBounds [_selectedSprite];
			if (currentSprite.textureOffset != value) {
				currentSprite.textureOffset = value;
			}
		}
		get {
			return MySpriteAtlas.spriteBounds [_selectedSprite].textureOffset;
		}
	}

	private Vector2 CurrentSpriteTextureScale {
		set {
			SpriteBounds currentSprite = MySpriteAtlas.spriteBounds [_selectedSprite];
			if (currentSprite.textureTiling != value) {
				currentSprite.textureTiling = value;
			}
		}
		get {
			return MySpriteAtlas.spriteBounds [_selectedSprite].textureTiling;
		}
	}

	private float CurrentSpriteScale {
		set {
			SpriteBounds currentSprite = MySpriteAtlas.spriteBounds [_selectedSprite];
			if (currentSprite.spriteSizeRatio != value) {
				currentSprite.spriteSizeRatio = value;
			}
		}
		get {
			return MySpriteAtlas.spriteBounds [_selectedSprite].spriteSizeRatio;
		}
	}
	#endregion
	
	/// <summary>
	/// Link to target object
	/// </summary>
	/// <value>
	/// My sprite atlas.
	/// </value>
	protected SpriteAtlas MySpriteAtlas {
		get {
			if (_mySpriteAtlas == null) {
				_mySpriteAtlas = target as SpriteAtlas;
			}
			return _mySpriteAtlas;
		}
	}
	
	protected Material MyFramesSequence {
		get {
			return MySpriteAtlas.atlas;
		}
		set {
			MySpriteAtlas.atlas = value;
		}
	}
	
	public override void OnInspectorGUI ()
	{
		if (MySpriteAtlas.spriteBounds.Count > 0) {
			DrawSpriteEditor ();
			DrawSpritePreview ();
		} else {
			Color oldColor = GUI.color;
			GUI.color = Color.red;
			EditorGUILayout.LabelField ("ERROR");
			GUI.color = oldColor;
		}
	}

	private void DrawSpriteEditor ()
	{
		string[] spritesNames = new string[MySpriteAtlas.spriteBounds.Count];
		for (int i = 0; i< spritesNames.Length; i++) {
			spritesNames [i] = MySpriteAtlas.spriteBounds [i].name;
			
		}
		_selectedSprite = EditorGUILayout.Popup ("Sprite Name", _selectedSprite, spritesNames);
		
		DrawSpriteProperties ();
	}

	void DrawSpriteProperties ()
	{
		CurrentSpriteName = EditorGUILayout.TextField ("Name", CurrentSpriteName);
		CurrentSpriteTextureOffset = EditorGUILayout.Vector2Field ("Texture offset", CurrentSpriteTextureOffset);
		CurrentSpriteTextureScale = EditorGUILayout.Vector2Field ("Texture scale", CurrentSpriteTextureScale);
		CurrentSpriteScale = EditorGUILayout.FloatField ("Scale", CurrentSpriteScale);
	}

	public void DrawSpritePreview ()
	{
		SpriteBounds currentSprite = MySpriteAtlas.spriteBounds [_selectedSprite];
		
		if (currentSprite != null && currentSprite.textureTiling.x != 0) {
			EditorGUILayout.Separator ();
		
			isPreviewSettingsExpanded = EditorGUILayout.Foldout (isPreviewSettingsExpanded, "Preview Settings");
			if (isPreviewSettingsExpanded) {
				MEEditorTools.isToDrawPreview = EditorGUILayout.Toggle ("Is to draw preview", MEEditorTools.isToDrawPreview);
				MEEditorTools.isToDrawPreviewBG = EditorGUILayout.Toggle ("Is to draw preview background grid", MEEditorTools.isToDrawPreviewBG);
				MEEditorTools.previewBGDensity = EditorGUILayout.IntSlider ("Preview BG grid density", MEEditorTools.previewBGDensity, 1, 64);
				MEEditorTools.checkersColor1 = EditorGUILayout.ColorField ("Preview BG grid color 1", MEEditorTools.checkersColor1);
				MEEditorTools.checkersColor2 = EditorGUILayout.ColorField ("Preview BG grid color 2", MEEditorTools.checkersColor2);
			}
			
			if (MEEditorTools.isToDrawPreview) {
				Rect rect = GUILayoutUtility.GetLastRect ();
				GUILayout.Space (rect.yMin + 12f + (Screen.width - 32f) * currentSprite.textureTiling.y / currentSprite.textureTiling.x);
					
				if (MEEditorTools.isToDrawPreviewBG) {
					MEEditorTools.DrawCheckersBG (
						new Rect (
							16f,
							rect.yMin + 24f,
							Screen.width - 32f,
							(Screen.width - 32f) * currentSprite.textureTiling.y / currentSprite.textureTiling.x
						),
						new Vector2 (MySpriteAtlas.atlas.mainTexture.width * currentSprite.textureTiling.x / (MEEditorTools.previewBGDensity), MySpriteAtlas.atlas.mainTexture.height * currentSprite.textureTiling.y / (MEEditorTools.previewBGDensity)),
						MEEditorTools.checkersColor1,
						MEEditorTools.checkersColor2
					);
				}
				
				GUI.DrawTextureWithTexCoords (
					new Rect (
						16f,
						rect.yMin + 24f,
						Screen.width - 32f,
						(Screen.width - 32f) * currentSprite.textureTiling.y / currentSprite.textureTiling.x
					),
					MySpriteAtlas.atlas.mainTexture,
					new Rect (
						currentSprite.textureOffset.x,
						currentSprite.textureOffset.y,
						currentSprite.textureTiling.x,
						currentSprite.textureTiling.y
					)
				);
				
				MEEditorTools.DrawSpriteOutline (
					new Rect (
						16f,
						rect.yMin + 24f,
						Screen.width - 32f,
						(Screen.width - 32f) * currentSprite.textureTiling.y / currentSprite.textureTiling.x
					),
					new Color (0.4f, 1f, 0f, 1f)
				);
				
				EditorGUI.DropShadowLabel (
					new Rect (
							0f,
							rect.yMin + 26f + (Screen.width - 32f) * currentSprite.textureTiling.y / currentSprite.textureTiling.x,
							Screen.width,
							24f
					),
					currentSprite.name + "\n" + 
					MySpriteAtlas.atlas.mainTexture.width * currentSprite.textureTiling.x + "x" + 
					MySpriteAtlas.atlas.mainTexture.height * currentSprite.textureTiling.y
				);
			}
		}
	}
}
