using UnityEngine;
using System.Collections;

public class ScreenDimensionsHelper : MonoBehaviour {

	[SerializeField]
	Camera uiCamera = null;
	
	[SerializeField]
	UIRoot uiRoot = null;
	
	float uiRootScale = 1f;

	public float UIRootScale { get { return this.uiRootScale; } }
	
	Bounds localScreenBounds;
	Bounds worldScreenBounds;

	public Bounds LocalScreenBounds {
		get {
			return this.localScreenBounds;
		}
	}

	public Bounds WorldScreenBounds {
		get {
			return this.worldScreenBounds;
		}
	}
	
	void Update() {

		float height = uiRoot.manualHeight;
		float width = height * ((float)Screen.width/Screen.height);

		uiRootScale = uiRoot.transform.localScale.x;

		Vector3 localSize = new Vector3( width, height, 0f );
		Vector3 worldSize = uiRootScale * localSize;
		
		localScreenBounds = new Bounds( Vector3.zero, localSize );
		worldScreenBounds = new Bounds( Vector3.zero, worldSize ); 
	}
	
	public Bounds GetBoundsRelativeTo( Transform t ) {
		Vector3 min = t.InverseTransformPoint( worldScreenBounds.min );
		Vector3 max = t.InverseTransformPoint( worldScreenBounds.max );
		Bounds b = new Bounds( min, Vector3.zero );
		b.Encapsulate( max );
		return b;
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireCube( uiCamera.transform.position, worldScreenBounds.size );
	}

}
