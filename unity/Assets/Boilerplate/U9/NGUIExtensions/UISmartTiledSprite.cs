using UnityEngine;
using System.Collections;

public class UISmartTiledSprite : UISprite {

	[SerializeField]
	Vector2 tileOffset = new Vector2( 0f, 0f );

	public Vector2 TileOffset {
		get {
			return tileOffset;
		}
		set {
			if (tileOffset != value) {
				tileOffset = value;
				MarkAsChanged ();
			}
		}
	}

	protected override void Awake ()
	{
		base.Awake ();
		type = Type.Tiled;
	}
	
	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture tex = material.mainTexture;
		if (tex == null) return;

		Rect rect = mInnerUV;

		Vector2 scale = cachedTransform.localScale;
		float pixelSize = atlas.pixelSize;
		float width = Mathf.Abs(rect.width / scale.x) * pixelSize;
		float height = Mathf.Abs(rect.height / scale.y) * pixelSize;

		// Safety check. Useful so Unity doesn't run out of memory if the sprites are too small.
		if (width < 0.01f || height < 0.01f)
		{
			Debug.LogWarning("The tiled sprite (" + NGUITools.GetHierarchy(gameObject) + ") is too small.\nConsider using a bigger one.");

			width = 0.01f;
			height = 0.01f;
		}

		Vector2 min = new Vector2(rect.xMin / tex.width, rect.yMin / tex.height);
		Vector2 max = new Vector2(rect.xMax / tex.width, rect.yMax / tex.height);
		Vector2 clipped = max;

		Color colF = color;
		Color32 col = atlas.premultipliedAlpha ? NGUITools.ApplyPMA(colF) : colF;


		bool finalX = false, finalY = false;


		float x = 0f, y = 0f, x2 = 0f, y2 = 0f;

		float widthWrapper = 10000f * width, heightWrapper = 10000f * height;

		Vector2 absoluteTileOffset = new Vector2 ( -tileOffset.x * width, tileOffset.y * height);

		while (y < 1f && !finalY)
		{

			float relY = (y - pivotOffset.y);
			float tileY =  Wrap( relY + absoluteTileOffset.y, height, 0f ) / height;
			if ( Mathf.Approximately( tileY, 1f ) ) {
				tileY = 0f;
			}

			float uvY = Mathf.Lerp (min.y, max.y, tileY);

			if (y == 0f) {
				y2 = y + (1f - tileY) * height;
				clipped.y = max.y;
			} else {
				clipped.y = uvY + (max.y-min.y);
			}

			if (y2 > 1f) {
				y2 = 1f;
				tileY =  Wrap( 0.5f + absoluteTileOffset.y, height, 0f ) / height;
				clipped.y = Mathf.Lerp (min.y, max.y, tileY );
				finalY = true;
			} 

			finalX = false;

			x = 0f;

			while (x < 1f && !finalX )
			{

				float relX = (x + pivotOffset.x);
				float tileX = Wrap( relX + absoluteTileOffset.x, width, 0f ) / width;
				if ( Mathf.Approximately( tileX, 1f ) ) {
					tileX = 0f;
				}

				float uvX = Mathf.Lerp (min.x, max.x, tileX);

				if( x == 0f ) {
					x2 = x + (1f - tileX) * width;
					clipped.x = max.x;
				} else {
					clipped.x = uvX + (max.x-min.x);
				}

				if (x2 > 1f) {
					x2 = 1f;
					tileX =  Wrap( 0.5f + absoluteTileOffset.x, width, 0f ) / width;
					clipped.x = Mathf.Lerp (min.x, max.x, tileX );
					finalX = true;
				} 

				verts.Add(new Vector3(x2, -y, 0f));
				verts.Add(new Vector3(x2, -y2, 0f));
				verts.Add(new Vector3(x, -y2, 0f));
				verts.Add(new Vector3(x, -y, 0f));

				//Debug.Log ("X: " + x + ", X2: " + x2 + ", Y: " + y + ", Y2: " + y2 + ", FINALX: " + finalX + ", FINALY: " + finalY );


				uvs.Add(new Vector2(clipped.x, 1f - uvY));
				uvs.Add(new Vector2(clipped.x, 1f - clipped.y));
				uvs.Add(new Vector2(uvX, 1f - clipped.y));
				uvs.Add(new Vector2(uvX, 1f - uvY));

				cols.Add(col);
				cols.Add(col);
				cols.Add(col);
				cols.Add(col);

				x = x2;
				x2 += width;
			}
			y = y2;
			y2 += height;
		}

	}

	float Wrap(  float n, float max, float min ) {
		if( n > 0.0f ) {
			return ( n % max ) + min;
		}
		else {
			return max - ( Mathf.Abs( n ) % max ) + min;
		}
	}


}
