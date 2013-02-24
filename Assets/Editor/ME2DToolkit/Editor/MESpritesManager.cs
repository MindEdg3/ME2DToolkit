using UnityEngine;
using UnityEditor;
using System.Collections;

public class MESpritesManager : EditorWindow
{
	public FramesMap frames;
	public AnimationSequence framesSequence;
	public string errorMessage = "";
	private ObjectType selectedObjectOption;
	private float scaleMultiplier = 1f;
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
			DrawScaleMultiplier ();
			break;
		case ObjectType.AnimatedSprite:
			DrawScaleMultiplier ();
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
	private void DrawScaleMultiplier ()
	{
		float _scaleMultiplier = EditorGUILayout.FloatField ("Scale Multiplier", scaleMultiplier);
		if (_scaleMultiplier > 0) {
			scaleMultiplier = _scaleMultiplier;
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
		newGO.active = false;
		
		switch (oType) {
		case ObjectType.SimpleSprite:
			newGO.name = "Simple 2D Sprite";
			MESprite newSS = newGO.AddComponent<MESprite> ();
			break;
		case ObjectType.AnimatedSprite:
			if (framesSequence == null) {
				DestroyImmediate (newGO);
				errorMessage = "Set FramesSequence!";
				return;
			} else {
				newGO.name = "Animated Sprite";
				AnimatedSprite newAS = newGO.AddComponent<AnimatedSprite> ();
				newAS.framesSequence = this.framesSequence;
				MeshRenderer graphics = new GameObject ("graphics").AddComponent<MeshRenderer> ();
				graphics.gameObject.AddComponent<MeshFilter> ();
				graphics.transform.parent = newGO.transform;
				graphics.castShadows = false;
				graphics.receiveShadows = false;
				newGO.name = "Animation (" + framesSequence.gameObject.name + ")";
				newAS.renderTarget = graphics;
				break;
			}
		}
		
		errorMessage = "";
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