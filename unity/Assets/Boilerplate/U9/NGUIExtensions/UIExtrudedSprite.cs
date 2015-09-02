using UnityEngine;
using System.Collections;

public class UIExtrudedSprite : UISprite {
	
	[HideInInspector] [SerializeField] float extrusionDepth = 1f;

	public float ExtrusionDepth {
		get {
			return this.extrusionDepth;
		}
		set {
			if( extrusionDepth != value ) {
				extrusionDepth = value;
				MarkAsChanged();
			}
		}
	}	
	
	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		base.OnFill (verts, uvs, cols);
		
		
		Vector2 uv0 = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 uv1 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		
		verts.Add(new Vector3(1f,  0f, 0f));
		verts.Add(new Vector3(1f,  0f, extrusionDepth));
		verts.Add(new Vector3(1f,  -1f, extrusionDepth));
		verts.Add(new Vector3(1f,  -1f, 0f));
		
		verts.Add(new Vector3(1f,  0f, 0f));
		verts.Add(new Vector3(0f,  0f, 0f));
		verts.Add(new Vector3(0f,  0f, extrusionDepth));
		verts.Add(new Vector3(1f,  0f, extrusionDepth));
		
		verts.Add(new Vector3(0f,  -1f, 0f));
		verts.Add(new Vector3(0f,  -1f, extrusionDepth));
		verts.Add(new Vector3(0f,  0f, extrusionDepth));
		verts.Add(new Vector3(0f,  0f, 0f));
		
		verts.Add(new Vector3(1f,  -1f, extrusionDepth));
		verts.Add(new Vector3(0f,  -1f, extrusionDepth));
		verts.Add(new Vector3(0f,  -1f, 0f));
		verts.Add(new Vector3(1f,  -1f, 0f));
		
		for( int i = 0 ; i < 4 ; i++ ) {
			uvs.Add(uv1);
			uvs.Add(new Vector2(uv1.x, uv0.y));
			uvs.Add(uv0);
			uvs.Add(new Vector2(uv0.x, uv1.y));
		}
		
#if UNITY_3_5_4
		Color col = color;
#else
		Color32 col = color;
#endif
		for( int i = 0 ; i < 16 ; i++ ) {
			cols.Add(col);
		}
		
	}
}
