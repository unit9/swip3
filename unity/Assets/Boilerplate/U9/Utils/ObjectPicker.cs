using UnityEngine;
using System.Collections;

public class ObjectPicker  {

	  // Return the GameObject at the given screen position, or null if no valid object was found
    public static GameObject PickObject( Vector2 screenPos, Camera camera, LayerMask layers )
    {
        Ray ray = camera.ScreenPointToRay(screenPos);
        RaycastHit hit;
		
		Debug.DrawRay( ray.origin, ray.direction );
		
        if( Physics.Raycast( ray, out hit, float.MaxValue, layers ) )
            return hit.collider.gameObject;

        return null;
    }

    public static T PickComponent<T>( Vector2 screenPos, Camera camera, LayerMask layers ) where T : Component
    {
        GameObject go = PickObject( screenPos, camera, layers );
        
        if( !go )
            return default(T);

        return go.GetComponent<T>();
    }
	
	 public static T PickComponentAll<T>( Vector2 screenPos, Camera camera, LayerMask layers ) where T : Component
    {
		Ray ray = camera.ScreenPointToRay(screenPos);
		
        RaycastHit[] hits = Physics.RaycastAll( ray, Mathf.Infinity, layers );
		foreach( RaycastHit h in hits ) {
			T t = h.transform.GetComponent<T>();
			if( t ) {
				return t;
			}
		}
		
		return default(T);
    }
	
}
