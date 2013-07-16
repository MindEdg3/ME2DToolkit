using System;
using UnityEngine;

[ExecuteInEditMode]
/// <summary>
/// Runtime class for simple 2D sprite.
/// </summary>
public class MESprite : MonoBehaviour
{
	#region Fields
	[HideInInspector]
	[SerializeField]
	public string _spriteName = "";
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
	protected SpriteAtlas _spriteAtlas;
	protected bool isNeedToRefresh = false;
	[HideInInspector]
	[SerializeField]
	protected MeshFilter _renderTargetMeshFilter;
	[HideInInspector]
	[SerializeField]
	public SpriteBounds previousBoundaries = new SpriteBounds ("", Vector2.zero, Vector2.zero, 0f);
	[HideInInspector]
	[SerializeField]
	public float previousScale = 0f;
	#endregion
	
	#region Properties
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
	/// Gets or sets sprites atlas.
	/// </summary>
	/// <value>
	/// This sprites atlas.
	/// </value>
	public SpriteAtlas MySpritesAtlas {
		get {
			return _spriteAtlas;
		}
		set {
			if (_spriteAtlas != value) {
				_spriteAtlas = value;
				
				// set valid frame name
				if (_spriteAtlas != null) {
					SpriteBounds _spriteBounds = _spriteAtlas.spriteBounds.Find (sb => sb.name == SpriteName);
					if (_spriteBounds == null) {
						SpriteName = _spriteAtlas.spriteBounds [0].name;
					}
				}
				isNeedToRefresh = true;
			}
		}
	}

	protected SpriteBounds SpriteBoundaries {
		get {
			SpriteBounds _spriteBoundaries = null;
			
			if (MySpritesAtlas != null) {
				_spriteBoundaries = MySpritesAtlas.spriteBounds.Find (sb => sb.name == SpriteName);
				if (_spriteBoundaries == null) {
					SpriteName = MySpritesAtlas.spriteBounds [0].name;
					_spriteBoundaries = MySpritesAtlas.spriteBounds [0];
				}
			}
			
			return _spriteBoundaries;
		}
	}
	
	/// <summary>
	/// Gets or sets the name of the current frame.
	/// </summary>
	/// <value>
	/// The name of the current frame.
	/// </value>
	public string SpriteName {
		get {
			return _spriteName;
		}
		set {
			if (_spriteName != value && MySpritesAtlas != null) {
				SpriteBounds _spriteBounds = MySpritesAtlas.spriteBounds.Find (sb => sb.name == value);
				if (_spriteBounds != null) {
					_spriteName = value;
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
	#endregion
	
	public virtual void OnEnable ()
	{
	}
	
	// Use this for initialization
	public virtual void Start ()
	{
		if (MySpritesAtlas != null) {
			RenderTargetMeshFilter.sharedMesh = CreateMesh (SpriteBoundaries);
		}
	}
	
	// LateUpdate is called once per frame after all updates
	public virtual void LateUpdate ()
	{
		if (Application.isPlaying) {
			if (isNeedToRefresh) {
				RefreshSprite ();
				isNeedToRefresh = false;
			}
		} else {
			if (RenderTargetMeshFilter.sharedMesh == null && MySpritesAtlas != null) {
				RenderTargetMeshFilter.sharedMesh = CreateMesh (SpriteBoundaries);
			}
		}
	}
	
	public virtual void OnDestroy ()
	{
		// Destroying old mesh
		if (RenderTargetMeshFilter.sharedMesh != null) {
			if (Application.isPlaying) {
				Destroy (RenderTargetMeshFilter.mesh);
			} else {
				DestroyImmediate (RenderTargetMeshFilter.sharedMesh);
			}
		}
	}
	
	public virtual void OnApplicationQuit ()
	{
		if (RenderTargetMeshFilter.sharedMesh != null) {
			DestroyImmediate (RenderTargetMeshFilter.sharedMesh);
		}
	}
	
	public virtual void RefreshSprite ()
	{
		if (MySpritesAtlas == null) {
#if UNITY_EDITOR
			if (!Application.isPlaying)
				DestroyImmediate(RenderTargetMeshFilter.sharedMesh);
			else
#endif
			Destroy (RenderTargetMeshFilter.sharedMesh);
		} else {
			if (RenderTarget.sharedMaterial != MySpritesAtlas.atlas) {
				RenderTarget.sharedMaterial = MySpritesAtlas.atlas;
			}
			SpriteBounds currentBoundaries = SpriteBoundaries;
			UpdateMesh (RenderTargetMeshFilter, currentBoundaries);
			previousBoundaries = currentBoundaries;
		}
	}

	public Mesh CreateMesh (SpriteBounds spriteBoundaries)
	{
		// new Mesh creating
		Mesh newMesh = new Mesh ();
		
#if UNITY_EDITOR
		newMesh.hideFlags = HideFlags.DontSave;
#endif
		
		// Object name
		newMesh.name = "plane_" + SpriteName;
		
		float halfWidth = Scale * 0.5f * spriteBoundaries.textureTiling.x * spriteBoundaries.spriteSizeRatio;
		float halfHeight = Scale * 0.5f * spriteBoundaries.textureTiling.y * spriteBoundaries.spriteSizeRatio;
		
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
				new Vector2 (spriteBoundaries.textureOffset.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureTiling.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureTiling.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureTiling.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureTiling.x, spriteBoundaries.textureOffset.y)
			};
		
		newMesh.triangles = new int[] {0,1,2,0,2,3};
			
		newMesh.normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};

		return newMesh;
	}
	
	public void UpdateMesh (MeshFilter targetMeshFilter, SpriteBounds spriteBoundaries)
	{
		if (targetMeshFilter.sharedMesh == null) {	
			targetMeshFilter.sharedMesh = CreateMesh (spriteBoundaries);
		} else {
			if (previousBoundaries.textureTiling.x != spriteBoundaries.textureTiling.x ||
				previousBoundaries.textureTiling.y != spriteBoundaries.textureTiling.y ||
				previousBoundaries.spriteSizeRatio != spriteBoundaries.spriteSizeRatio ||
				isNeedToRefresh) {
				
				float halfWidth = Scale * 0.5f * spriteBoundaries.textureTiling.x * spriteBoundaries.spriteSizeRatio;
				float halfHeight = Scale * 0.5f * spriteBoundaries.textureTiling.y * spriteBoundaries.spriteSizeRatio;
		
				targetMeshFilter.sharedMesh.vertices = new Vector3[] {
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
			
				targetMeshFilter.sharedMesh.uv = new Vector2[] {
				spriteBoundaries.textureOffset,
				new Vector2 (spriteBoundaries.textureOffset.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureTiling.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureTiling.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureTiling.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureTiling.x, spriteBoundaries.textureOffset.y)
			};
			}
			isNeedToRefresh = true;
		}
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
