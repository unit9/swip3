using UnityEngine;
using System.Collections;

public class FullScreenPanelBounds : MonoBehaviour {
	
	[SerializeField]
	UIPanel panel = null;
	
	[SerializeField]
	Vector2 centerPoint = Vector3.zero;

	[SerializeField]
	ScreenDimensionsHelper screenDimensionsHelper = null;

	void Reset() {
		panel = GetComponent<UIPanel>();
	}
	
	// Use this for initialization
	void Start () {
		UpdateBounds();
	}

	public void UpdateBounds() {
		Bounds localScreenBounds = screenDimensionsHelper.LocalScreenBounds;
		panel.clipRange = new Vector4( centerPoint.x * localScreenBounds.size.x, centerPoint.y * localScreenBounds.size.y, localScreenBounds.size.x, localScreenBounds.size.y );
	}

}
