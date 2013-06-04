using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;

public class SpriteAtlas : MonoBehaviour
{
//	[HideInInspector]
	[SerializeField]
	public Material atlas;
//	[HideInInspector]
	[SerializeField]
	public List<SpriteBounds> spriteBounds;
}

/// <summary>
/// Sprite bounds, that primarilly used to keep information about scale and offset of texture. Also keeps it's name and size ratio.
/// </summary>
[Serializable]
public class SpriteBounds
{
	public string name;
	public Vector2 textureOffset = Vector2.zero;
	public Vector2 textureTiling = Vector2.zero;
	public float spriteSizeRatio = 1f;
	
	private SpriteBounds ()
	{
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="SpriteBounds"/> class.
	/// </summary>
	/// <param name='name'>
	/// Sprite name.
	/// </param>
	/// <param name='offset'>
	/// Sprite offset.
	/// </param>
	/// <param name='scale'>
	/// Sprite scale.
	/// </param>
	/// <param name='spriteSizeRatio'>
	/// Sprite size ratio from 1024x1024 pixels.
	/// </param>
	public SpriteBounds (string name, Vector2 offset, Vector2 scale, float spriteSizeRatio)
	{
		this.name = name;
		this.textureOffset = offset;
		this.textureTiling = scale;
		this.spriteSizeRatio = spriteSizeRatio;
	}
	
	/// <summary>
	/// Serves as a hash function for a <see cref="SpriteBounds"/> object.
	/// </summary>
	/// <returns>
	/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.
	/// </returns>
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
	
	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="SpriteBounds"/>.
	/// </summary>
	/// <param name='other'>
	/// The <see cref="System.Object"/> to compare with the current <see cref="SpriteBounds"/>.
	/// </param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="SpriteBounds"/>;
	/// otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals (object other)
	{
		SpriteBounds otherSB;
		
		otherSB = other as SpriteBounds;
		
		if (otherSB == null) {
			return false;
		} else {
			return Equals (otherSB);
		}
	}
	
	/// <summary>
	/// Determines whether the specified <see cref="SpriteBounds"/> is equal to the current <see cref="SpriteBounds"/>.
	/// </summary>
	/// <param name='otherSB'>
	/// The <see cref="SpriteBounds"/> to compare with the current <see cref="SpriteBounds"/>.
	/// </param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="SpriteBounds"/> is equal to the current <see cref="SpriteBounds"/>;
	/// otherwise, <c>false</c>.
	/// </returns>
	public bool Equals (SpriteBounds otherSB)
	{
		return (otherSB.name.Equals (name)) && (this.textureOffset.Equals (otherSB.textureOffset)) && (textureTiling.Equals (otherSB.textureTiling)) && (spriteSizeRatio.Equals (spriteSizeRatio));
	}
}