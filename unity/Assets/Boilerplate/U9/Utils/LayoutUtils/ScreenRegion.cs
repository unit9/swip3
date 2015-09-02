using UnityEngine;
using System.Collections;

public class ScreenRegion : MonoBehaviour
{

	[SerializeField]
	float marginLeft = 0f, marginRight = 0f, marginTop = 0f, marginBottom = 0f;

	public float MarginLeft { get { return marginLeft; } set { marginLeft = value; } }
	public float MarginRight { get { return marginRight; } set { marginRight = value; } }
	public float MarginTop { get { return marginTop; } set { marginTop = value; } }
	public float MarginBottom { get { return marginBottom; } set { marginBottom = value; } }

	[SerializeField]
	Camera camera = null;
	Transform cachedCameraTransform;

	Bounds localBounds, worldBounds;

	public Bounds LocalBounds {
		get {
			return localBounds;
		}
	}

	public Bounds WorldBounds {
		get {
			return worldBounds;
		}
	}

	const float kMaxScaleDistance = 1f;

	[SerializeField]
	ScreenDimensionsHelper screenDimensionsHelper;

	void Start() {
		if (camera) {
			cachedCameraTransform = camera.transform;
		}
	}

	public void Update() {
		localBounds = screenDimensionsHelper.LocalScreenBounds;
		if( camera ) {
			localBounds.center += cachedCameraTransform.localPosition;
			localBounds.size *= camera.orthographicSize;
		}

		localBounds.max -= ( camera ? camera.orthographicSize : 1f ) * new Vector3( marginRight, marginTop, 0f );
		localBounds.min += ( camera ? camera.orthographicSize : 1f ) * new Vector3( marginLeft, marginBottom, 0f );



		Vector3 center = localBounds.center;
		center.z = transform.position.z;
		localBounds.center = center;

		worldBounds = screenDimensionsHelper.WorldScreenBounds;
		if( camera ) {
			worldBounds.center += cachedCameraTransform.position;
			worldBounds.size *= camera.orthographicSize;
		}
		
		worldBounds.max -= ( camera ? camera.orthographicSize : 1f ) * transform.lossyScale.x * new Vector3( marginRight, marginTop, 0f );
		worldBounds.min += ( camera ? camera.orthographicSize : 1f ) * transform.lossyScale.x * new Vector3( marginLeft, marginBottom, 0f );

		center = worldBounds.center;
		center.z = transform.position.z;
		worldBounds.center = center;
		
	}

	/// <summary>
	/// Clamps a transform to this screen region and rotates it to face the aimPos
	/// </summary>
	/// <returns> distance between clamped position and aimPos</returns>
	/// <param name="transformToClamp">Transform to clamp.</param>
	/// <param name="aimPos">Aim position.</param>
	public float ClampToScreenRegion( Vector3 aimPos, out Vector3 clampedPos, out Vector3 clampedDirection ) {
		aimPos.z = worldBounds.center.z;

//		Bounds cameraBounds = worldBounds;
//		cameraBounds.center += camera.transform.position;
//		cameraBounds.size *= camera.orthographicSize;

		if( !worldBounds.Contains(aimPos) ) {



			Vector3 origin = worldBounds.center;
			origin.z = aimPos.z;
			Vector3 direction = (worldBounds.center-aimPos).normalized;
			Ray r = new Ray( worldBounds.center - 100f*direction, direction );
			float distance = 0f;

			clampedDirection = direction;

			if( worldBounds.IntersectRay( r, out distance ) ) {
				clampedPos = r.GetPoint(distance);
				return Vector3.Distance(clampedPos,aimPos);
			} else {
				Debug.LogWarning("ClampToScreenRegion ray missed bounds :(");
			}
		}

		clampedDirection = Vector3.up;
		clampedPos = aimPos;
		//transformToClamp.rotation = Quaternion.Slerp( transformToClamp.rotation, Quaternion.identity, 5f*Time.smoothDeltaTime );

		return 0f;
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube( worldBounds.center, worldBounds.size );
	}
	
}

