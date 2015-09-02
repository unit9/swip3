using UnityEngine;
using System.Collections;

public class SetUIRootHeight : MonoBehaviour {

	[SerializeField]
	UIRoot uiRoot = null;

	[SerializeField]
	Transform gameGrid = null;

	[SerializeField]
	UISprite upperHUDBackground = null, lowerHUDBackground = null;

	/// <summary>
	/// Sets the height of the UIRoot so that the game matches with width of the screen.
	/// Also centers the game between the UI at the top and bottom of the screen, if it exists.
	/// </summary>
	void Update () 
	{
		float aspectRatio = (float)Screen.height / Screen.width;

		uiRoot.manualHeight =  Mathf.FloorToInt( 320f * aspectRatio );

		uiRoot.manualHeight += uiRoot.manualHeight % 2;

		float lowerBound = -uiRoot.manualHeight;
		float upperBound = uiRoot.manualHeight;

		if (upperHUDBackground.gameObject.activeInHierarchy) {
			upperBound -= upperHUDBackground.height;
		}

		if (lowerHUDBackground.gameObject.activeInHierarchy) {
			lowerBound += lowerHUDBackground.height;
		}
		gameGrid.transform.localPosition = new Vector3(0, -58.6321f, 0);
		//gameGrid.transform.localPosition = new Vector3 (0f, Mathf.Lerp ( lowerBound, upperBound , 0.5f), 0f);
	}

}
