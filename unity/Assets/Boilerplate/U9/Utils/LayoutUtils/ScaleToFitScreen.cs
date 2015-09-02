using UnityEngine;
using System.Collections;

public class ScaleToFitScreen : MonoBehaviour {

	[SerializeField]
	UIRoot root = null;

	float aspectRatio;

	// Use this for initialization
	void Start () {
		aspectRatio = transform.localScale.x / transform.localScale.y;
	}
	
	// Update is called once per frame
	void Update () {

		float height = 0.5f * root.manualHeight;

		float screenRatio = (float)Screen.width / Screen.height;
		if ( screenRatio > aspectRatio) {
			transform.localScale = new Vector3 (screenRatio * height, height * ( screenRatio / aspectRatio ), 0f);
		} else {
			transform.localScale = new Vector3 (aspectRatio * height, height, 0f);
		}



	}
}
