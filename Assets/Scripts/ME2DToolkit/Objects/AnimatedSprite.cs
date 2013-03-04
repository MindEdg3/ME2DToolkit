using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MESprite
{
	public AnimationSequence framesSequence;
	public float speed = 1.0f;
	private float lastTimeFrameChanged;
	private int frameIndex;
	
	public bool IsAnimationEnded {
		get {
			return frameIndex == framesSequence.frames.Count - 1;
		}
	}

	// Use this for initialization
	public override void Start ()
	{		
		base.Start ();
	}
	
	// Update is called once per frame
	public void FUpdate ()
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
			
			AdjustTexture (frameIndex);
			
			lastTimeFrameChanged = Time.time;
		}
	}
	
	private void AdjustTexture (int adjustableFrameIndex)
	{
		// set frame sprite to renderer
		SpriteBounds currentBoundaries = MyFramesMap.spriteBounds.Find (sb => sb.name == framesSequence.frames [adjustableFrameIndex].frameName);
		
		if (!currentBoundaries.Equals (lastBoundaries)) {
			// Destroying old mesh
			if (RenderTargetMeshFilter.sharedMesh != null) {
#if UNITY_EDITOR
				DestroyImmediate (RenderTargetMeshFilter.sharedMesh);
#else
				Destroy (RenderTargetMeshFilter.sharedMesh);
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
				currentBoundaries.textureOffset,
				new Vector2 (currentBoundaries.textureOffset.x, currentBoundaries.textureOffset.y + currentBoundaries.textureScale.y),
				new Vector2 (currentBoundaries.textureOffset.x + currentBoundaries.textureScale.x, currentBoundaries.textureOffset.y + currentBoundaries.textureScale.y),
				new Vector2 (currentBoundaries.textureOffset.x + currentBoundaries.textureScale.x, currentBoundaries.textureOffset.y)
			};
		
			newMesh.triangles = new int[] {0,1,2,0,2,3};
			
			newMesh.normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
			
			RenderTargetMeshFilter.sharedMesh = newMesh;
		}
	}
	
	public void SetAnimation (AnimationSequence framesSequence)
	{
		this.framesSequence = framesSequence;
		frameIndex = 0;
	}
	
	public void FadeAnimation (AnimationSequence framesSequence)
	{
		if (this.MyFramesMap.atlas != MyFramesMap.atlas) {
			for (int i = 0; i < framesSequence.frames.Count; i++) {
				if (framesSequence.frames [i].frameName == this.framesSequence.frames [frameIndex].frameName) {
					this.framesSequence = framesSequence;
					frameIndex = i;
					break;
				}
			}
		}
		this.framesSequence = framesSequence;
		frameIndex = 0;
	}
}