using UnityEngine;
using System.Collections;

public static class TransformUtils
{

	public static void SwitchParentAndMaintainScale( Transform targetTransform, Transform parentTransform ) {
		Vector3 oldLocalScale = targetTransform.localScale;
		targetTransform.parent = parentTransform;
		targetTransform.localScale = oldLocalScale;	
	}
	
	public static string GetTransformPath( Transform t ) {
		string path = t.name;
		while( t.parent ) {
			t = t.parent;
			path = t.name + "/" + path;
		}
		return path;
	}
	
}

