using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIPerspectivePixelPerfect : MonoBehaviour {

	[SerializeField]
	Camera targetCamera;
	
	[SerializeField] UIRoot root;
	
	void Reset() {
	
		root = NGUITools.FindInParents<UIRoot>(gameObject);
		targetCamera = NGUITools.FindInParents<Camera>(gameObject);
		Debug.Log ("ROOT: " + root );
	}
	
	void OnEnable() {
		Vector2 screenPoint = targetCamera.WorldToScreenPoint( transform.position + targetCamera.transform.right );
		screenPoint -= new Vector2( Screen.width/2, Screen.height/2 );	
		float rootScale = root.transform.localScale.x;
		transform.localScale = ( Vector3.one / screenPoint.x ) / rootScale;	
	}
	
}
