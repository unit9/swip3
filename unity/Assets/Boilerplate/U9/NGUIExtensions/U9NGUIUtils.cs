using UnityEngine;
using System.Collections;

public class U9NGUIUtils : MonoBehaviour {

	public static Bounds CalculateTransformLocalBounds( Transform trans ) {
		UIWidget[] widgets = trans.GetComponentsInChildren<UIWidget> (true);

		Bounds b = new Bounds ();
		foreach (UIWidget w in widgets) {
			Bounds bounds = CalculateWidgetLocalBounds (w,trans);
		//	Debug.Log ("Widget: " + w + ", Bounds= " + bounds );
			b.Encapsulate (bounds);
		}

		return b;
	}

	public static Bounds CalculateWidgetLocalBounds( UIWidget w, Transform relativeTo ) {
		float width = w.cachedTransform.localScale.x;
		float height = w.cachedTransform.localScale.y;

		Vector2 pivotOffset = w.pivotOffset;
		return new Bounds( GetLocalOffset( w.transform, relativeTo ) + new Vector3( 0.5f*pivotOffset.x*width, 0.5f*pivotOffset.y*height, 0f ), new Vector3( width, height, 0f ) );
	}

	public static Vector3 GetLocalOffset( Transform trans, Transform relativeTo ) {
		Transform currentTransform = trans;
		Vector3 offset = Vector3.zero;
		while ( currentTransform && currentTransform != relativeTo) {
			offset += currentTransform.localPosition;
			currentTransform = currentTransform.parent;
		}
		return offset;
	}

}
