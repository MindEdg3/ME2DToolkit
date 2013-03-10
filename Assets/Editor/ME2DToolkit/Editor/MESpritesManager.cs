using UnityEngine;
using UnityEditor;
using System.Collections;

public class MESpritesManager : EditorWindow
{
	public AnimationSequence framesSequence;
	public string errorMessage = "";
	private ObjectType selectedObjectOption;
	private SpriteHorizontalAlignment horizontalSpriteAlignment;
	private SpriteVerticalAlignment verticalSpriteAlignment;
	private float scale = 1f;
	private float speed = 1f;
	private FramesMap framesMap;
	private int selectedFrameNameIndex;
	private string frameName;
	private Texture2D atlas;
	private Vector2 scrollPosition;
	
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
		
		scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
		///
		/// Objects
		/// 
		GUILayout.Label ("Add object");
		
		selectedObjectOption = (ObjectType)EditorGUILayout.EnumPopup ("Object preset", selectedObjectOption);
		
		switch (selectedObjectOption) {
		case ObjectType.SimpleSprite:
			DrawFrameMap ();
			DrawFrameName ();
			DrawScale ();
			DrawAlignment ();
			break;
		case ObjectType.AnimatedSprite:
			DrawFrameMap ();
			DrawAlignment ();
			DrawScale ();
			DrawAnimSequenceSelector ();
			DrawSpeed ();
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
		if (GUILayout.Button ("Bake Scales on selection (Shift+B)")) {
			MESpritesManager.BakeSelected ();
		}
		
		EditorGUILayout.EndScrollView ();
	}
	
	#region GUI elements
	private void DrawFrameMap ()
	{
		framesMap = EditorGUILayout.ObjectField ("Frame Map", framesMap, typeof(FramesMap), false) as FramesMap;
	}
	
	private void DrawFrameName ()
	{
		if (framesMap != null) {
			string[] frameNames = new string[framesMap.spriteBounds.Count];
			for (int i = 0; i< frameNames.Length; i++) {
				frameNames [i] = framesMap.spriteBounds [i].name;
				/*if (frameNames [i].Equals (FrameName)) {
					selectedFrameNameIndex = i;
				}*/
			}
			selectedFrameNameIndex = EditorGUILayout.Popup ("Frame Name", selectedFrameNameIndex, frameNames);
			frameName = frameNames [selectedFrameNameIndex];
		}
	}
	
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
	
	private void DrawSpeed ()
	{
		float _speed = EditorGUILayout.FloatField ("Speed", speed);
		if (_speed > 0) {
			speed = _speed;
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
			newSS.MyFramesMap = framesMap;
			newSS.FrameName = frameName;
			newSS.HorizontalSpriteAlignment = horizontalSpriteAlignment;
			newSS.VerticalSpriteAlignment = verticalSpriteAlignment;
			newSS.Scale = scale;
			break;
		case ObjectType.AnimatedSprite:
			newGO.name = "Animated Sprite";
			AnimatedSprite newAS = newGO.AddComponent<AnimatedSprite> ();
			newAS.RenderTarget = graphics;
			newAS.MyFramesMap = framesMap;
			newAS.HorizontalSpriteAlignment = horizontalSpriteAlignment;
			newAS.VerticalSpriteAlignment = verticalSpriteAlignment;
			newAS.Scale = scale;
			newAS.Speed = speed;
			newAS.FramesSequence = framesSequence;
			break;
		}
		
		errorMessage = "";
		Selection.activeGameObject = newGO;
		newGO.active = true;
		Undo.RegisterCreatedObjectUndo (newGO, "Adding New Sprite");
	}
	
	/// <summary>
	/// Bakes() local scale of selected sprite objects.
	/// </summary>
	[MenuItem ("Window/MEAnimation/Bake Scales on selection #b")]
	static void BakeSelected ()
	{
		Object[] sprites = Selection.GetFiltered (typeof(MESprite), SelectionMode.TopLevel);
		/*
		 * Undo is not working correctly, because of refreshing, that refreshes only one selected object
		 * 
		Transform[] spriteGOs = new Transform[sprites.Length];
		for (int i =0; i < sprites.Length; i++) {
			spriteGOs [i] = (sprites [i] as MESprite).transform;
		}
		
		Object[] undoObjects = new Object[sprites.Length + spriteGOs.Length];
		spriteGOs.CopyTo(undoObjects, 0);
		sprites.CopyTo(undoObjects, spriteGOs.Length);
		
		Undo.RegisterUndo (undoObjects, "Baking Scales on selection");
		*/
		
		if (sprites.Length > 0) {
			for (int i = 0; i < sprites.Length; i++) {
				BakeScale (sprites [i] as MESprite);
			}
		}
	}
	
	/// <summary>
	/// Bakes local scale of sprite.
	/// </summary>
	/// <param name='spriteObject'>
	/// Target sprite object.
	/// </param>
	public static void BakeScale (MESprite spriteObject)
	{
		Vector3 oldScale = spriteObject.transform.localScale;
		
		if (oldScale.x <= oldScale.y) {
			spriteObject.transform.localScale = new Vector3 (1f, oldScale.y / oldScale.x, 1f);
			spriteObject.Scale *= oldScale.x;
		} else {
			spriteObject.transform.localScale = new Vector3 (oldScale.x / oldScale.y, 1f, 1f);
			spriteObject.Scale *= oldScale.y;
		}
#if UNITY_EDITOR
		if (!Application.isPlaying){
			spriteObject.RefreshSprite();
		}
#endif
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
				mWhiteTex = MESpritesManager.CreateDummyTex ();
			return mWhiteTex;
		}
	}
	#endregion
}

enum ObjectType
{
	SimpleSprite,
	AnimatedSprite
}