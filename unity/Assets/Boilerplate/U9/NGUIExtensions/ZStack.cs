using UnityEngine;
using System.Collections.Generic;

public class ZStack : MonoBehaviour {

	[SerializeField]
	float interval = 0.1f;
	
	class TransformToZMap {
		internal Transform Transform { get; set; }
		internal float Z { get; set; }
	}
	
	List<TransformToZMap> transformToZMaps = new List<TransformToZMap>();

	public float GetNewZ( Transform t, float minimum = 0f ) {

		TransformToZMap existing = transformToZMaps.Find ((obj) => {
			return (obj.Transform == t); });

		if (existing != null) {
			existing.Z = GetNextZ (t, minimum);
			return existing.Z;
		} else {
			float newZ = GetNextZ (t, minimum);
			transformToZMaps.Add (new TransformToZMap () { Transform = t, Z = newZ });
			return newZ;
		}

	}

	public float GetNextZ( Transform t, float minimum ) {
		float maxZ = minimum;
		foreach( TransformToZMap m in transformToZMaps ) {
			if( m.Z > maxZ ) {
				maxZ = m.Z;
			}
		}
		maxZ += interval;
		return maxZ;
	}

	public void RemoveZ( Transform t ) {
		transformToZMaps.RemoveAll( (obj) => { return (obj.Transform == t); } );
	}


#if UNITY_EDITOR
//	void OnGUI() {
//		GUILayout.Space (200f);
//		GUI.color = Color.blue;
//		GUILayout.Label ("Z Stack count: " + transformToZMaps.Count);
//		if (transformToZMaps.Count > 0) {
//			GUILayout.Label ("Z: " + transformToZMaps [transformToZMaps.Count - 1].Z);
//		}
//	}

#endif
}
