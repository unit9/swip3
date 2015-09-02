using UnityEngine;
using System.Collections;

public static class GizmoUtils {

	const float arrowLength = 0.03f;
	
	public static void DrawArrow( Vector3 from, Vector3 to, Vector3 ortho ) {
		
		Vector3 direction = (to-from).normalized;
		Vector3 cross = Vector3.Cross( direction, ortho ).normalized;
		
		Gizmos.DrawLine( from, to );
		
		Vector3 halfway = Vector3.Lerp( from, to, 0.333f );
		
		Gizmos.DrawLine( halfway, halfway + cross*arrowLength - direction*arrowLength );
		Gizmos.DrawLine( halfway, halfway - cross*arrowLength - direction*arrowLength );
		
	}
	
}
