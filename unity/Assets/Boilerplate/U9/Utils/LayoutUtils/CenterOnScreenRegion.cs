using UnityEngine;
using System.Collections;

public class CenterOnScreenRegion : MonoBehaviour {

	[SerializeField]
	ScreenRegion screenRegion = null;
	

	// Update is called once per frame
	void Update () {
		Vector3 pos = screenRegion.LocalBounds.center;
		pos.z = transform.localPosition.z;
		transform.localPosition = pos;
	}
}
