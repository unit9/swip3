using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UITexture))]
public class TextureResourceView : MonoBehaviour
{
	[SerializeField]
	UITexture
		uiTexture = null;

	[SerializeField]
	bool makePixelPerfect = false;

	[SerializeField]
	float pixelScale = 1f;

	[SerializeField]
	bool instantLoad = false;

	[SerializeField]
	string shinyShader = "Custom/Shiny Shader";
	
	Texture2D texture = null;

	void Reset() {
		uiTexture = GetComponent<UITexture> ();
	}

	void Awake() {
		uiTexture.enabled = false;
	}

	bool loaded = false;

	void HandleShinyTextureLoaded( Object o ) {
		Texture2D tex = (Texture2D)o;
		if(uiTexture.material != null)
			uiTexture.material.SetTexture ("_OffsetTex", tex);
	}

	string resourcePath = null;
	public string ResourcePath {
		get {
			return this.resourcePath;
		}
		set {	
			if (this.resourcePath != value) {
				if (this.resourcePath != null) {
					uiTexture.mainTexture = null;
					if (loaded) {
												loaded = false;
							ResourceManager.Instance.UnloadResource (this.resourcePath, HandleTextureLoadComplete);
					}
					texture = null;
					uiTexture.enabled = false;
				}
				this.resourcePath = value;
				if ( !string.IsNullOrEmpty(this.resourcePath) ) {
										loaded = true;
					if (instantLoad) {
						HandleTextureLoadComplete( ResourceManager.Instance.LoadResource (this.resourcePath) );
					} else {
						ResourceManager.Instance.AddToLoadQueue (this.resourcePath, HandleTextureLoadComplete);
					}
				} else {
					uiTexture.enabled = false;
				}

			}
		}
	}

	void HandleTextureLoadComplete (Object obj)
	{
	//	Debug.Log ("HandleTextureLoadComplete: " + obj.name );
		if( obj && uiTexture ) {
			this.texture = (Texture2D)obj;
			uiTexture.material = null;
			uiTexture.enabled = true;
			uiTexture.mainTexture = texture;
			SetUITextureZoomRect(zoomRect);
			if (makePixelPerfect) {
				uiTexture.MakePixelPerfect ();
				uiTexture.cachedTransform.localScale *= pixelScale;
			}
		} else {
			Debug.LogError ("Null texture loaded!? Was expecting something from: " + resourcePath );
		}
	}

	Rect zoomRect = new Rect( 0f, 0f, 1f, 1f );

	public Rect ZoomRect {
		set {
			//Debug.Log ("SET ZOOM RECT: " + value);
			zoomRect = value;
			// NGUI doesn't like it if there's no texture and you try to access the material
			if( this.texture ) {

				SetUITextureZoomRect (value);
			}
		}
	}

	void SetUITextureZoomRect (Rect value)
	{
		uiTexture.uvRect = value;
		//uiTexture.material.mainTextureScale = new Vector2 (value.width, value.height);
		//uiTexture.material.mainTextureOffset = new Vector2 (value.x, value.y);
	}

	public void SetUITextureShader(Shader value, bool resetColour)
	{
		uiTexture.shader = value;
		if(resetColour)
			uiTexture.color = Color.white;
	}

	public UIWidget Widget {
		get {
			return uiTexture;
		}
	}

//	void OnViewDisplayed ()
//	{
//		uiTexture.mainTexture = texture;
//	}
	
}

