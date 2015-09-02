using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIFogOfWar : UISprite, ISerializable {
	

	[SerializeField]
	AnimationCurve falloffCurve;

	float[,] targetAlphas, alphas;

	const float uScale = 10f, vScale = 10f;

	int uLength, vLength;
	Vector2[,] uv0s;// = new Vector2[uvStretch,uvStretch];
	Vector2[,] uv1s;// = new Vector2[uvStretch,uvStretch];
	Vector2[,] uv2s;// = new Vector2[uvStretch,uvStretch];
	Vector2[,] uv3s;// = new Vector2[uvStretch,uvStretch];

	bool initedUVs = false;

	Material fogMaterial;

	const float gridSize = 200f;

	int gridWidth, gridHeight;

	protected override void Awake ()
	{
		base.Awake ();
//		Debug.Log ("FOG INIT: " + transform.localScale );
//
//		Debug.Log ("FOG ATLAS: " + atlas.replacement);
//		Debug.Log ("FOG MATERIAL: " + atlas.replacement.spriteMaterial);
//		Debug.Log ("FOG TEXTURE: " + atlas.replacement.spriteMaterial.mainTexture );

		gridWidth = Mathf.FloorToInt( transform.localScale.x / gridSize );
		gridHeight = Mathf.FloorToInt( transform.localScale.y / gridSize );

		targetAlphas = new float[gridWidth,gridHeight];
		alphas = new float[gridWidth,gridHeight];

		for (int i = 0, ni = targetAlphas.GetLength(0) ; i < ni ; i++) {
			for (int j = 0, nj = targetAlphas.GetLength(1) ; j < nj ; j++) {
				targetAlphas[i,j] = 1f;
				alphas[i,j] = 1f;
			}
		}
	}

	void OnEnable() {

		StartCoroutine( UpdateAlphasCoroutine() );
		StartCoroutine( UpdateScrollCoroutine() );
	}

	IEnumerator UpdateScrollCoroutine() {
		while (true) {
			if (atlas.spriteMaterial) {
				float t = (0.1f * Time.time);
				float scrollA = 0.25f * t + 0.05f * Mathf.Sin (1.5f * t);
				float scrollB = 0.05f * t + 0.01f * Mathf.Sin (t);
				atlas.spriteMaterial.SetFloat ("_ScrollA", scrollA);
				atlas.spriteMaterial.SetFloat ("_ScrollB", scrollB);
			}
			yield return null;
		}
	}

	IEnumerator UpdateAlphasCoroutine() {
		yield return null;

		while( true ) {
			bool dirty = false;

			for (int i = 0, ni = targetAlphas.GetLength(0) ; i < ni ; i++) {
				for (int j = 0, nj = targetAlphas.GetLength(1) ; j < nj ; j++) {
					float a = alphas[i,j];
					float target = targetAlphas [i, j];
					if( a-target > 0.01f ) {
						float newAlpha = Mathf.Lerp( a, target, 2f*Time.smoothDeltaTime );
						alphas[i,j] = newAlpha;
						dirty = true;
					}
				}
			}
			if( dirty ) {
				MarkAsChanged();
			}
			yield return null;
		}
	}

	public IEnumerator RevealCircle( Vector3 worldPos, float radius, AnimationCurve falloffCurve ) {

		// This actually gives you coordinates from -0.5f to 0.5f because the transform is scaled. Lucky!
		Vector3 localPos = cachedTransform.InverseTransformPoint( worldPos );
		localPos.z = 0f;

		Vector3 localScale = cachedTransform.localScale;


		float radiusWidth = (radius/localScale.x);
		float radiusHeight = (radius/localScale.y);

		float x = 0.5f+localPos.x;
		float xMin = x-radiusWidth;
		float xMax = x+radiusWidth;

		float y = 1f-(0.5f+localPos.y);
		float yMin = y-radiusHeight;
		float yMax = y+radiusHeight;

		int xGridMin = Mathf.FloorToInt( xMin*(gridWidth-1) ) ;
		int xGridMax = Mathf.CeilToInt( xMax*(gridWidth-1) ) ;
		int yGridMin = Mathf.FloorToInt( yMin*(gridHeight-1) ) ;
		int yGridMax = Mathf.CeilToInt( yMax*(gridHeight-1) );

		float xStep = 2f*radius / ( xGridMax-xGridMin );
		float yStep = 2f*radius / ( yGridMin-yGridMax );


		Vector3 currentPos = new Vector3( localPos.x-radius , 0f, 0f );

		
		for (int i = xGridMin, ni = xGridMax ; i < ni ; i++) {
			currentPos.y = localPos.y + radius;
			for (int j = yGridMin, nj = yGridMax ; j < nj ; j++) {
				if( i >= 0 && i < targetAlphas.GetLength(0) && j >= 0 && j < targetAlphas.GetLength(1) ) {
					float distance = Vector3.Distance( currentPos, localPos );
					targetAlphas[i,j] = Mathf.Min ( Mathf.Clamp01( falloffCurve.Evaluate(distance/radius) ), targetAlphas[i,j] );
					currentPos.y += yStep;
				}

			}
			//yield return null;
			currentPos.x += xStep;
		}
		yield return null;
	//	MarkAsChanged();

	}

	protected override void OnUpdate ()
	{
		base.OnUpdate();

		if ( mChanged && !initedUVs )
		{
			initedUVs = true;

			Texture tex = mainTexture;
		
			mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, tex.width, tex.height);

			float gridXDistance = transform.localScale.x / gridWidth;
			float gridYDistance = transform.localScale.y / gridHeight;

			uLength = Mathf.RoundToInt(  gridXDistance / uScale );
			vLength = Mathf.RoundToInt( gridYDistance / vScale );

			uv0s= new Vector2[uLength,vLength];
			uv1s= new Vector2[uLength,vLength];
			uv2s= new Vector2[uLength,vLength];
			uv3s= new Vector2[uLength,vLength];

			for (int i = 0  ; i < uLength ; i++) {
				for (int j = 0  ; j < vLength ; j++) {
					float xInner = (float)i/(uLength);
					float yInner = (float)j/(vLength);
					float xOuter = (float)(i+1)/(uLength);
					float yOuter = (float)(j+1)/(vLength);
				
					uv0s[i,j] = new Vector2( Mathf.Lerp( mOuterUV.xMin, mOuterUV.xMax, xInner ), Mathf.Lerp( mOuterUV.yMin, mOuterUV.yMax, yInner ) );
					uv1s[i,j] = new Vector2( Mathf.Lerp( mOuterUV.xMin, mOuterUV.xMax, xOuter ), Mathf.Lerp( mOuterUV.yMin, mOuterUV.yMax, yInner ) );
					uv2s[i,j] = new Vector2( Mathf.Lerp( mOuterUV.xMin, mOuterUV.xMax, xOuter ), Mathf.Lerp( mOuterUV.yMin, mOuterUV.yMax, yOuter ) );
					uv3s[i,j] = new Vector2( Mathf.Lerp( mOuterUV.xMin, mOuterUV.xMax, xInner ), Mathf.Lerp( mOuterUV.yMin, mOuterUV.yMax, yOuter ) );
					
				}
			}

		}

	}

	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{

		for (int i = 0, ni = alphas.GetLength(0)-1 ; i < ni ; i++) {
			float x0 = (float)i/ni;
			float x1 = (float)(i+1)/ni;
			for (int j = 0, nj = alphas.GetLength(1)-1 ; j < nj ; j++) {
				float y0 = -(float)j/nj;
				float y1 = -(float)(j+1)/nj;

				verts.Add( new Vector3(x0,y0,0f) );
				verts.Add( new Vector3(x1,y0,0f) );
				verts.Add( new Vector3(x1,y1,0f) );
				verts.Add( new Vector3(x0,y1,0f) );

				uvs.Add( uv0s[i%uLength,j%vLength] );
				uvs.Add( uv1s[i%uLength,j%vLength] );
				uvs.Add( uv2s[i%uLength,j%vLength] );
				uvs.Add( uv3s[i%uLength,j%vLength] );

				//Debug.Log ( string.Format ("uv3={0}, uv1={1}, uv2={2}, uv0={3}", uv3.ToString("N4"), uv1.ToString("N4"), uv2.ToString("N4"), uv0.ToString("N4")) );
		
//				if( Application.isEditor ) {
//					cols.Add( new Color(1f,1f,1f,0) );
//					cols.Add( new Color(1f,1f,1f,0) );
//					cols.Add( new Color(1f,1f,1f,0) );
//					cols.Add( new Color(1f,1f,1f,0) );
//				} else {
					cols.Add( new Color(1f,1f,1f,alphas[i,j]) );
					cols.Add( new Color(1f,1f,1f,alphas[i+1,j]) );
					cols.Add( new Color(1f,1f,1f,alphas[i+1,j+1]) );
					cols.Add( new Color(1f,1f,1f,alphas[i,j+1]) );
//				}

			}
		}
	}

	#region ISerializable implementation
	public Hashtable Serialize ()
	{
		Hashtable t = new Hashtable();
		ArrayList list = new ArrayList(gridWidth*gridHeight);
		for (int i = 0, ni = targetAlphas.GetLength(0) ; i < ni ; i++) {
			for (int j = 0, nj = targetAlphas.GetLength(1) ; j < nj ; j++) {
				list.Add( (double)targetAlphas[i,j] );
			}
		}
		t.Add( "TargetAlphas", new ArrayList(list) );
		return t;
	} 
	public void Deserialize (Hashtable data)
	{
		if( data.ContainsKey("TargetAlphas") ) {
			ArrayList list = (ArrayList)data["TargetAlphas"];
			IEnumerator e = list.GetEnumerator();
			for (int i = 0, ni = targetAlphas.GetLength(0) ; i < ni ; i++) {
				for (int j = 0, nj = targetAlphas.GetLength(1) ; j < nj ; j++) {
					if( e.MoveNext() ) {
						this.targetAlphas[i,j] = (float)(double)e.Current;
					}
				}
			}
		}
	}
	public string ID {
		get {
			throw new System.NotImplementedException ();
		}
	}

	public void Unload() {

	}
	#endregion
}
