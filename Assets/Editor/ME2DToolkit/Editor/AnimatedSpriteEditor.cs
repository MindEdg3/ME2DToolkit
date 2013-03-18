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
	
	protected AnimationSequence MyFramesSequence {
		get {
			return MyAnimatedSprite.FramesSequence;
		}
		set {
			MyAnimatedSprite.FramesSequence = value;
			if (value != null) {
				int frameInd = MyAnimatedSprite.frameIndex;
				if (value.frames.Count > frameInd) {
					if (!value.frames [frameInd].framesMap.Equals (MyFramesMap)) {
						MyFramesMap = value.frames [frameInd].framesMap;
					}
					FrameName = value.frames [frameInd].frameName;
				} else {
					if (!value.frames [0].framesMap.Equals (MyFramesMap)) {
						MyFramesMap = value.frames [0].framesMap;
					}
					FrameName = value.frames [0].frameName;
				}
			}
		}
	}
	
	public override void OnInspectorGUI ()
	{
		DrawFramesMap ();
		
		if (MyFramesMap != null) {
			DrawFramesSequence ();
			DrawSpeed ();
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
	
	protected virtual void DrawFramesSequence ()
	{
		MyFramesSequence = EditorGUILayout.ObjectField ("Frames Sequence", MyAnimatedSprite.FramesSequence, typeof(AnimationSequence), false) as AnimationSequence;
	}
	
	protected virtual void DrawSpeed ()
	{
		MyAnimatedSprite.Speed = EditorGUILayout.FloatField ("Speed", MyAnimatedSprite.Speed);
	}
}
