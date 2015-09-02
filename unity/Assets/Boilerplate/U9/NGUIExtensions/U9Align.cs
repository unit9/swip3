using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class U9Align : MonoBehaviour {

	[SerializeField]
	bool alignX = true, alignY = true, alignZ = true;

	[SerializeField]
	Vector3 pivot = Vector3.zero;

	[SerializeField]
	float lerp = 0f;

	Transform cachedTransform;

	void Start() {
		cachedTransform = transform;
	}

	void LateUpdate () {
		Bounds b = NGUIMath.CalculateRelativeWidgetBounds (cachedTransform);
		Vector3 pos = cachedTransform.localPosition;
		if (alignX) {
			pos.x = -b.center.x - pivot.x*b.size.x;
		}
		if (alignY) {
			pos.y = -b.center.y - pivot.y*b.size.y;
		}
		if (alignZ) {
			pos.z = -b.center.z - pivot.z*b.size.z;
		}

		if (lerp > 0f && !Application.isEditor) {
			cachedTransform.localPosition = Vector3.Lerp (cachedTransform.localPosition, pos, Time.smoothDeltaTime * lerp);
		} else {
			cachedTransform.localPosition = pos;
		}

	}

}
