using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextureResourceView))]
public class LoadTextureOnEnable : MonoBehaviour {

	[SerializeField]
	TextureResourceView textureResourceView = null;

	[SerializeField]
	string textureToLoad = "";

	void Reset() {
		textureResourceView = GetComponent<TextureResourceView> ();
	}

	void OnEnable() {
		//Debug.Log ("LOAD TEXTURE: " + gameObject);
		textureResourceView.ResourcePath = textureToLoad;
	}

	void OnDisable() {
		//		Debug.Log ("UNLOAD TEXTURE: " + gameObject);
		textureResourceView.ResourcePath = null;
	}

	void OnDestroy() {
		textureResourceView.ResourcePath = null;
	}

}