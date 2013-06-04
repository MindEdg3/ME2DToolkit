using UnityEngine;
using UnityEditor;
using System.Collections;

public class MEEditorTools
{
#region Settings
	[SerializeField]
	public static bool isToDrawPreview = true;
	[SerializeField]
	public static bool isToDrawPreviewBG = true;
	[SerializeField]
	public static int previewBGDensity = 1;
	[SerializeField]
	public static Color checkersColor1 = Color.white;
	[SerializeField]
	public static Color checkersColor2 = Color.grey;
#endregion
	
	static Texture2D mWhiteTex;
	static Texture2D mCheckersTex;

	/// <summary>
	/// Returns a blank usable 1x1 white texture.
	/// </summary>
	static public Texture2D BlankTexture {
		get {
			if (mWhiteTex == null) {
				mWhiteTex = CreateDummyTex ();
			}
			return mWhiteTex;
		}
	}
	/// <summary>
	/// Returns a blank usable 1x1 white texture.
	/// </summary>
	static public Texture2D CheckersTexture {
		get {
			if (mCheckersTex == null) {
				mCheckersTex = CreateCheckersTex (Color.white, Color.grey);
			}
			return mCheckersTex;
		}
	}
	
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
	/// Create colored checkers texture
	/// </summary>
	static Texture2D CreateCheckersTex (Color color1, Color color2)
	{
		Texture2D tex = new Texture2D (2, 2);
		tex.name = "[Generated] Checkers Texture";
		tex.hideFlags = HideFlags.DontSave;
		tex.filterMode = FilterMode.Point;
		tex.SetPixel (0, 0, color1);
		tex.SetPixel (1, 1, color1);
		tex.SetPixel (1, 0, color2);
		tex.SetPixel (0, 1, color2);
		tex.Apply ();
		return tex;
	}

	/// <summary>
	/// Draw a single-pixel outline around the specified rectangle.
	/// </summary>
	static public void DrawSpriteOutline (Rect rect, Color color)
	{
		if (Event.current.type == EventType.Repaint) {
			Texture2D tex = BlankTexture;
			GUI.color = color;
			GUI.DrawTexture (new Rect (rect.xMin, rect.yMin, 1f, rect.height), tex);
			GUI.DrawTexture (new Rect (rect.xMax, rect.yMin, 1f, rect.height), tex);
			GUI.DrawTexture (new Rect (rect.xMin, rect.yMin, rect.width, 1f), tex);
			GUI.DrawTexture (new Rect (rect.xMin, rect.yMax, rect.width, 1f), tex);
			GUI.color = Color.white;
		}
	}
	
	static public void DrawCheckersBG (Rect position, Vector2 resolution, Color color1, Color color2)
	{
		
		if (Event.current.type == EventType.Repaint) {
			Texture2D tex = CreateCheckersTex (color1, color2);
			GUI.DrawTextureWithTexCoords (position, tex, new Rect (0f, 0f, resolution.x * 0.5f, resolution.y * 0.5f));
		}
	}
}
