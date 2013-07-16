using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Animated Sprite editor GUI for inspector.
/// </summary>
[CustomEditor(typeof(AnimatedSprite))]
[CanEditMultipleObjects]
public class AnimatedSpriteEditor : SimpleSpriteEditor
{
	protected bool isToAnimatePreview = false;
	protected AnimatedSprite _myAnimatedSprite;
	
	/// <summary>
	/// Link to target object
	/// </summary>
	/// <value>
	/// My sprite.
	/// </value>
	protected AnimatedSprite MyAnimatedSprite {
		get {
			if (_myAnimatedSprite == null) {
				_myAnimatedSprite = target as AnimatedSprite;
			}
			return _myAnimatedSprite;
		}
	}
	
	protected AnimationSequence MySpritesSequence {
		get {
			return MyAnimatedSprite.SpritesSequence;
		}
		set {
			MyAnimatedSprite.SpritesSequence = value;
			if (value != null) {
				int spriteIndex = MyAnimatedSprite.spriteIndex;
				if (value.sprites.Count > spriteIndex) {
					SpriteAtlas sa = value.sprites [spriteIndex].spritesAtlas;
					if (sa != null) {
						if (!sa.Equals (MySpritesAtlas)) {
							MySpritesAtlas = value.sprites [spriteIndex].spritesAtlas;
						}
					}
					SpriteName = value.sprites [spriteIndex].spriteName;
				} else {
					if (!value.sprites [0].spritesAtlas.Equals (MySpritesAtlas)) {
						MySpritesAtlas = value.sprites [0].spritesAtlas;
					}
					SpriteName = value.sprites [0].spriteName;
				}
			}
		}
	}
	
	public override void OnInspectorGUI ()
	{
		DrawSpriteAtlas ();
		
		if (MySpritesAtlas != null) {
			DrawSpritesSequence ();
			DrawSpeed ();
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
	
	protected virtual void DrawSpritesSequence ()
	{
		MySpritesSequence = EditorGUILayout.ObjectField ("Sprites Sequence", MyAnimatedSprite.SpritesSequence, typeof(AnimationSequence), false) as AnimationSequence;
	}
	
	protected virtual void DrawSpeed ()
	{
		MyAnimatedSprite.Speed = EditorGUILayout.FloatField ("Speed", MyAnimatedSprite.Speed);
	}
}
