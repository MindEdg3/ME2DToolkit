using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimatedSprite : MESprite
{
	public AnimationSequence framesSequence;
	public MeshRenderer renderTarget;
	public float speed = 1.0f;
	public AnimationSpriteHorizontalAnchor horizontalSpriteAnchor = AnimationSpriteHorizontalAnchor.Middle;
	public AnimationSpriteVerticalAnchor verticalSpriteAnchor = AnimationSpriteVerticalAnchor.Center;
	private float lastTimeFrameChanged;
	private int frameIndex;
	private bool isNeedToChangeSprite;
	private MeshFilter renderTargetMeshFilter;
	private SpriteBounds lastBoundaries = new SpriteBounds ("", Vector2.zero, Vector2.zero, 0f);
	
	public bool IsAnimationEnded {
		get {
			return frameIndex == framesSequence.frames.Count - 1;
		}
	}

	// Use this for initialization
	public override void Start ()
	{		
		if (renderTarget == null) {
			renderTarget = transform.GetComponentInChildren<MeshRenderer> ();
		}
		renderTarget.sharedMaterial = framesSequence.framesMap.atlas;
		renderTargetMeshFilter = renderTarget.GetComponent<MeshFilter> ();
		
		AdjustTexture (0);
	}
	
	void OnEnable ()
	{
		Start ();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) {
			AdjustTexture (0);
		}
#endif
		if ((lastTimeFrameChanged + 1 / speed / framesSequence.speed / framesSequence.frames [frameIndex].speed) < Time.time) {
			// switch to next frame.
			frameIndex++;
			if (frameIndex >= framesSequence.frames.Count) {
				frameIndex = 0;
			}
			
			// change sprite if it needed
			if (isNeedToChangeSprite) {
				renderTarget.sharedMaterial = framesSequence.framesMap.atlas;
				isNeedToChangeSprite = false;
			}
			
			AdjustTexture (frameIndex);
			
			lastTimeFrameChanged = Time.time;
		}
	}
#if UNITY_EDITOR
	void OnDestroy () {
		if (renderTargetMeshFilter != null){
			if (renderTargetMeshFilter.sharedMesh != null) {
				DestroyImmediate (renderTargetMeshFilter.sharedMesh);
			}
		}
	}
#endif
	
	private void AdjustTexture (int adjustableFrameIndex)
	{
		// set frame sprite to renderer
		SpriteBounds currentBoundaries = framesSequence.framesMap.spriteBounds.Find (sb => sb.name == framesSequence.frames [adjustableFrameIndex].frameName);
		
		if (!currentBoundaries.Equals (lastBoundaries)) {
			// Destroying old mesh
			if (renderTargetMeshFilter.sharedMesh != null) {
#if UNITY_EDITOR
				DestroyImmediate (renderTargetMeshFilter.sharedMesh);
#else
				Destroy (renderTargetMeshFilter.sharedMesh);
#endif
			}
			
			// new Mesh creating
			Mesh newMesh = new Mesh ();
			
			// Object name
			newMesh.name = "2D Plane";
			
			float halfWidth = 0.5f * currentBoundaries.textureScale.x * currentBoundaries.spriteSizeRatio;
			float halfHeight = 0.5f * currentBoundaries.textureScale.y * currentBoundaries.spriteSizeRatio;

			newMesh.vertices = new Vector3[] {
				new Vector3 (
					-1 * (1 + (int)horizontalSpriteAnchor) * halfWidth,
					-1 * (1 + (int)verticalSpriteAnchor) * halfHeight,
					0f
				),
				new Vector3 (
					-1 * (1 + (int)horizontalSpriteAnchor) * halfWidth,
					(1 - (int)verticalSpriteAnchor) * halfHeight,
					0f
				),
				new Vector3 (
					(1 - (int)horizontalSpriteAnchor) * halfWidth,
					(1 - (int)verticalSpriteAnchor) * halfHeight,
					0f
				),
				new Vector3 (
					(1 - (int)horizontalSpriteAnchor) * halfWidth,
					-1 * (1 + (int)verticalSpriteAnchor) * halfHeight,
					0f
				)
			};
			
			newMesh.uv = new Vector2[] {
				currentBoundaries.textureOffset,
				new Vector2 (currentBoundaries.textureOffset.x, currentBoundaries.textureOffset.y + currentBoundaries.textureScale.y),
				new Vector2 (currentBoundaries.textureOffset.x + currentBoundaries.textureScale.x, currentBoundaries.textureOffset.y + currentBoundaries.textureScale.y),
				new Vector2 (currentBoundaries.textureOffset.x + currentBoundaries.textureScale.x, currentBoundaries.textureOffset.y)
			};
		
			newMesh.triangles = new int[] {0,1,2,0,2,3};
			
			newMesh.normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
			
			renderTargetMeshFilter.sharedMesh = newMesh;
		}
	}
	
	public void SetAnimation (AnimationSequence framesSequence)
	{
		if (this.framesSequence.framesMap.atlas != framesSequence.framesMap.atlas) {
			isNeedToChangeSprite = true;
		}
		this.framesSequence = framesSequence;
		frameIndex = 0;
	}
	
	public void FadeAnimation (AnimationSequence framesSequence)
	{
		if (this.framesSequence.framesMap.atlas != framesSequence.framesMap.atlas) {
			for (int i = 0; i < framesSequence.frames.Count; i++) {
				if (framesSequence.frames [i].frameName == this.framesSequence.frames [frameIndex].frameName) {
					this.framesSequence = framesSequence;
					frameIndex = i;
					isNeedToChangeSprite = true;
					break;
				}
			}
			isNeedToChangeSprite = true;
		}
		this.framesSequence = framesSequence;
		frameIndex = 0;
	}
}

public enum AnimationSpriteHorizontalAnchor : int
{
	Left = -1,
	Middle = 0,
	Right = 1
}

public enum AnimationSpriteVerticalAnchor : int
{
	Bottom = -1,
	Center = 0,
	Top = 1
}