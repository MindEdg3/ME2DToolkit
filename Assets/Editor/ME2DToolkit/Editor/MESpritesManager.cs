using UnityEngine;
using UnityEditor;
using System.Collections;

public class MESpritesManager : EditorWindow
{
	public FramesMap frames;
	public AnimationSequence framesSequence;
	public string errorMessage = "";
	private ObjectType selectedObjectOption;
	private SpriteHorizontalAlignment horizontalSpriteAlignment;
	private SpriteVerticalAlignment verticalSpriteAlignment;
	private float scale = 1f;
	private FramesMap framesMap;
	private Texture2D atlas;
	
	[MenuItem("Window/MEAnimation/Sprites Manager %#m")]
	static void OpenWindow ()
	{
		EditorWindow.GetWindow<MESpritesManager> ("Sprites Manager");
	}
	
	/*void OnFocus ()
	{
		errorLabelStyle.name = "ErrorLabel";
		errorLabelStyle.normal.textColor = Color.red;
		errorLabelStyle.margin = new RectOffset (4, 4, 4, 4);
	}*/
	
	void OnGUI ()
	{
		
		///
		/// Objects
		/// 
		GUILayout.Label ("Add object");
		
		selectedObjectOption = (ObjectType)EditorGUILayout.EnumPopup ("Select Object", selectedObjectOption);
		
		switch (selectedObjectOption) {
		case ObjectType.SimpleSprite:
			DrawScale ();
			break;
		case ObjectType.AnimatedSprite:
			DrawScale ();
			DrawAnimSequenceSelector ();
			break;
		}
		GUI.color = Color.green;
		if (GUILayout.Button ("Add")) {
			CreateObject (selectedObjectOption);
		}
		
		GUI.color = Color.red;
		if (!string.IsNullOrEmpty (errorMessage)) {
			EditorGUILayout.Space ();
			GUILayout.Label ("" + errorMessage);
		}
		GUI.color = Color.white;
		
		EditorGUILayout.Space ();
		
		///
		/// Utils
		///
		GUILayout.Label ("Utils");
		if (GUILayout.Button ("Atlas Maker")) {
			EditorWindow.GetWindow (typeof(AtlasMaker));
		}
		if (GUILayout.Button ("Bake Scales (Shift+B)")) {
			MESpritesManager.BakeScales ();
		}
	}
	
	#region GUI elements
	private void DrawAlignment ()
	{
		horizontalSpriteAlignment = (SpriteHorizontalAlignment)EditorGUILayout.EnumPopup ("Horizontal Alignment", horizontalSpriteAlignment);
		verticalSpriteAlignment = (SpriteVerticalAlignment)EditorGUILayout.EnumPopup ("Vertical Alignment", verticalSpriteAlignment);
	}
	
	private void DrawScale ()
	{
		float _scale = EditorGUILayout.FloatField ("Scale", scale);
		if (_scale > 0) {
			scale = _scale;
		}
	}
	
	private void DrawAnimSequenceSelector ()
	{
		GameObject selectedSequenceGO = EditorGUILayout.ObjectField ("Animation Sequence:", framesSequence, typeof(GameObject), false) as GameObject;
		if (selectedSequenceGO != null) {
			framesSequence = selectedSequenceGO.GetComponent<AnimationSequence> ();
		}
	}
	#endregion
	
	void CreateObject (ObjectType oType)
	{
		GameObject newGO = new GameObject ();
		MeshRenderer graphics;
		newGO.active = false;
		graphics = newGO.AddComponent<MeshRenderer> ();
		newGO.AddComponent<MeshFilter> ();
		graphics.castShadows = false;
		graphics.receiveShadows = false;
		
		switch (oType) {
		case ObjectType.SimpleSprite:
			newGO.name = "Simple 2D Sprite";
			MESprite newSS = newGO.AddComponent<MESprite> ();
			newSS.RenderTarget = graphics;
			newSS.Scale = scale;
			break;
		case ObjectType.AnimatedSprite:
			if (framesSequence == null) {
				DestroyImmediate (newGO);
				errorMessage = "Set FramesSequence!";
				return;
			} else {
				newGO.name = "Animation (" + framesSequence.gameObject.name + ")";
				AnimatedSprite newAS = newGO.AddComponent<AnimatedSprite> ();
				newAS.framesSequence = this.framesSequence;
				newAS.RenderTarget = graphics;
				newAS.Scale = scale;
				break;
			}
		}
		
		errorMessage = "";
		Selection.activeGameObject = newGO;
		newGO.active = true;
	}
	
	[MenuItem ("Window/MEAnimation/Bake Scales #b")]
	static void BakeScales ()
	{
		Object[] sprites = Selection.GetFiltered (typeof(MESprite), SelectionMode.TopLevel);
		
		if (sprites.Length > 0) {
			for (int i = 0; i < sprites.Length; i++) {
				Debug.Log (sprites [i].name);
			}
		}
	}
}

enum ObjectType
{
	SimpleSprite,
	AnimatedSprite
}