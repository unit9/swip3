using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutHelper : MonoBehaviour {

	[System.Serializable]
	public class Layout {
		[SerializeField]
		string layoutName = "Layout";

		public string LayoutName {
			get {
				return layoutName;
			}
			set {
				layoutName = value;
			}
		}

		[SerializeField]
		Vector3[] localPositions, localScales;
		[SerializeField]
		Quaternion[] localRotations;

		public void CaptureLayout( Transform[] transforms ) {
			localPositions = new Vector3[transforms.Length];
			localScales = new Vector3[transforms.Length];
			localRotations = new Quaternion[transforms.Length];

			for (int i = 0; i < transforms.Length; i++) {
				Transform t = transforms [i];
				localPositions [i] = t.localPosition;
				localScales [i] = t.localScale;
				localRotations [i] = t.localRotation;
			}
		}

		public void EnforceLayout( Transform[] transforms ) {
			localPositions = new Vector3[transforms.Length];
			localScales = new Vector3[transforms.Length];
			localRotations = new Quaternion[transforms.Length];
			for (int i = 0; i < transforms.Length; i++) {
				Transform t = transforms [i];
				t.localPosition = localPositions [i];
				t.localScale = localScales [i];
				t.localRotation = localRotations [i];
			}
		}

	}

	[SerializeField]
	Layout[] layouts;

	public Layout[] Layouts {
		get {
			return layouts;
		}
		set {
			layouts = value;
		}
	}

	[SerializeField]
	Transform[] transforms;

	public Transform[] Transforms {
		get {
			return transforms;
		}
		set {
			transforms = value;
		}
	}

}
