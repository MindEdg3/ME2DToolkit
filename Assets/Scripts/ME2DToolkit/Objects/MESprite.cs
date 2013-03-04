using System;
using UnityEngine;

/// <summary>
/// Runtime class for simple 2D sprite.
/// </summary>
public class MESprite : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	public string _frameName = "";
	[HideInInspector]
	[SerializeField]
	public SpriteHorizontalAlignment _horizontalSpriteAlignment = SpriteHorizontalAlignment.Middle;
	[HideInInspector]
	[SerializeField]
	public SpriteVerticalAlignment _verticalSpriteAlignment = SpriteVerticalAlignment.Center;
	[HideInInspector]
	[SerializeField]
	public MeshRenderer _renderTarget;
	[HideInInspector]
	[SerializeField]
	protected float _scale = 1f;
	[HideInInspector]
	[SerializeField]
	protected FramesMap _framesMap;
	protected bool isNeedToRefresh = false;
	[HideInInspector]
	[SerializeField]
	protected MeshFilter _renderTargetMeshFilter;
	[HideInInspector]
	[SerializeField]
	public SpriteBounds lastBoundaries = new SpriteBounds ("", Vector2.zero, Vector2.zero, 0f);
	
	/// <summary>
	/// Gets or sets the scale of this sprite.
	/// </summary>
	/// <value>
	/// The scale of this sprite.
	/// </value>
	public float Scale {
		get {
			return _scale;
		}
		set {
			if (_scale != value) {
				_scale = Mathf.Clamp (value, 0, 1000);
				isNeedToRefresh = true;
#if UNITY_EDITOR
			if (!Application.isPlaying) 
				RefreshSprite();
#endif
			}
		}
	}
	
	/// <summary>
	/// Gets or sets frames map.
	/// </summary>
	/// <value>
	/// This frames map.
	/// </value>
	public FramesMap MyFramesMap {
		get {
			return _framesMap;
		}
		set {
			if (_framesMap != value) {
				_framesMap = value;
				
				// set valid frame name
				if (_framesMap != null) {
					SpriteBounds _spriteBounds = _framesMap.spriteBounds.Find (sb => sb.name == FrameName);
					if (_spriteBounds == null) {
						FrameName = _framesMap.spriteBounds [0].name;
					}
				}
				isNeedToRefresh = true;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the name of the current frame.
	/// </summary>
	/// <value>
	/// The name of the current frame.
	/// </value>
	public string FrameName {
		get {
			return _frameName;
		}
		set {
			if (_frameName != value) {
				SpriteBounds _spriteBounds = MyFramesMap.spriteBounds.Find (sb => sb.name == value);
				if (_spriteBounds != null) {
					_frameName = value;
					isNeedToRefresh = true;
				}
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the horizontal sprite alignment.
	/// </summary>
	/// <value>
	/// The horizontal sprite alignment.
	/// </value>
	public SpriteHorizontalAlignment HorizontalSpriteAlignment {
		get {
			return _horizontalSpriteAlignment;
		}
		set {
			if (_horizontalSpriteAlignment != value) {
				_horizontalSpriteAlignment = value;
				isNeedToRefresh = true;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the vertical sprite alignment.
	/// </summary>
	/// <value>
	/// The vertical sprite alignment.
	/// </value>
	public SpriteVerticalAlignment VerticalSpriteAlignment {
		get {
			return _verticalSpriteAlignment;
		}
		set {
			if (_verticalSpriteAlignment != value) {
				_verticalSpriteAlignment = value;
				isNeedToRefresh = true;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the render target.
	/// </summary>
	/// <value>
	/// The render target.
	/// </value>
	public MeshRenderer RenderTarget {
		get {
			if (_renderTarget == null) {
				_renderTarget = GetComponent<MeshRenderer> ();
			}
			return _renderTarget;
		}
		set {
			_renderTarget = value;
		}
	}
	
	/// <summary>
	/// Gets the render target mesh filter.
	/// </summary>
	/// <value>
	/// The render target mesh filter.
	/// </value>
	public MeshFilter RenderTargetMeshFilter {
		get {
			if (_renderTargetMeshFilter == null) {
				_renderTargetMeshFilter = RenderTarget.GetComponent<MeshFilter> ();
			}
			return _renderTargetMeshFilter;
		}
	}

	// Use this for initialization
	public virtual void Start ()
	{
	}
	
	// LateUpdate is called once per frame after all updates
	public virtual void LateUpdate ()
	{
		if (isNeedToRefresh) {
			isNeedToRefresh = false;
			RefreshSprite ();
		}
	}
	
	public virtual void OnDestroy ()
	{
		// Destroying old mesh
		if (RenderTargetMeshFilter.sharedMesh != null) {
#if UNITY_EDITOR
			if (!Application.isPlaying) 
				DestroyImmediate (RenderTargetMeshFilter.sharedMesh);
			else
#endif
			Destroy (RenderTargetMeshFilter.sharedMesh);
		}
	}
	
	public virtual void RefreshSprite ()
	{
		if (MyFramesMap == null) {			
#if UNITY_EDITOR
			if (!Application.isPlaying)
				DestroyImmediate(RenderTargetMeshFilter.sharedMesh);
			else
#endif
			Destroy (RenderTargetMeshFilter.sharedMesh);
		} else {
			if (RenderTarget.sharedMaterial != MyFramesMap.atlas) {
				RenderTarget.sharedMaterial = MyFramesMap.atlas;
			}
			
			// Destroying old mesh
			if (RenderTargetMeshFilter.sharedMesh != null) {
#if UNITY_EDITOR
				DestroyImmediate (RenderTargetMeshFilter.sharedMesh);
#else
				Destroy (RenderTargetMeshFilter.sharedMesh);
#endif
			}
		
			SpriteBounds currentBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == FrameName);
			RenderTargetMeshFilter.sharedMesh = CreateMesh (currentBoundaries);
		}
	}

	public Mesh CreateMesh (SpriteBounds spriteBoundaries)
	{
		// new Mesh creating
		Mesh newMesh = new Mesh ();
		
		// Object name
		newMesh.name = "plane_" + FrameName;
		
		float halfWidth = Scale * 0.5f * spriteBoundaries.textureScale.x * spriteBoundaries.spriteSizeRatio;
		float halfHeight = Scale * 0.5f * spriteBoundaries.textureScale.y * spriteBoundaries.spriteSizeRatio;
		
		newMesh.vertices = new Vector3[] {
				new Vector3 (
					-1 * (1 + (int)HorizontalSpriteAlignment) * halfWidth,
					-1 * (1 + (int)VerticalSpriteAlignment) * halfHeight,
					0f
				),
				new Vector3 (
					-1 * (1 + (int)HorizontalSpriteAlignment) * halfWidth,
					(1 - (int)VerticalSpriteAlignment) * halfHeight,
					0f
				),
				new Vector3 (
					(1 - (int)HorizontalSpriteAlignment) * halfWidth,
					(1 - (int)VerticalSpriteAlignment) * halfHeight,
					0f
				),
				new Vector3 (
					(1 - (int)HorizontalSpriteAlignment) * halfWidth,
					-1 * (1 + (int)VerticalSpriteAlignment) * halfHeight,
					0f
				)
			};
			
		newMesh.uv = new Vector2[] {
				spriteBoundaries.textureOffset,
				new Vector2 (spriteBoundaries.textureOffset.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureScale.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureScale.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureScale.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureScale.x, spriteBoundaries.textureOffset.y)
			};
		
		newMesh.triangles = new int[] {0,1,2,0,2,3};
			
		newMesh.normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
		
		return newMesh;
	}
}

public enum SpriteHorizontalAlignment : int
{
	Left = -1,
	Middle = 0,
	Right = 1
}

public enum SpriteVerticalAlignment : int
{
	Bottom = -1,
	Center = 0,
	Top = 1
}
