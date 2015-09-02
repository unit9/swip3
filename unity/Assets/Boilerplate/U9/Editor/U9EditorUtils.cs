using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class U9EditorUtils : MonoBehaviour {

	[MenuItem ("U9/Utils/Delete Player Prefs")]
	public static void DeletePlayerPrefs() {
		if( EditorUtility.DisplayDialog( "Delete Player Prefs?", "Are you sure you want to delete the player prefs?", "Yep!", "Bugger, no I don't!" ) ) {
			PlayerPrefs.DeleteAll();
		} else {
			if( !EditorUtility.DisplayDialog( "PHEW!", "Bet you're glad I added this confirm dialogue in now eh?", "Yep, I bow to Dave's greatness!", "I bow to nobody... but my prefs also get deleted!" ) ) {
				PlayerPrefs.DeleteAll();
			}
		}
	}
	
	[MenuItem ("U9/Utils/Localize UILabels")]
	public static void LocaliseUILabels() {
		Object[] uiLabels = FindObjectsOfTypeIncludingAssets(typeof(UILabel));
		foreach( Object o in uiLabels ) {
			UILabel label = o as UILabel;
			label.gameObject.AddComponent<UILocalizeLabel>();
		}
	}
	
	[MenuItem ("Assets/Snag resource path")]
	public static void SnagResourcePath() {
		Object o = Selection.activeObject;
		ClipboardHelper.clipBoard = AssetDatabase.GetAssetPath( o ).Replace("Assets/Decromancer/Resources/","").Replace(".prefab","".Replace(".psd",""));
	}
	
//	[MenuItem ("U9/Group Selected Objects %g")]
//	public static void GroupSelectedObjects() {
//		GameObject[] selectedObjects = Selection.gameObjects;
//		
//		GameObject group = new GameObject("Group");
//		Vector3 pos = Vector3.zero;
//		foreach( GameObject go in selectedObjects ) {
//			pos += go.transform.position;
//			Vector3 scale = go.transform.localScale;
//			go.transform.parent = group.transform;
//			go.transform.localScale = scale;
//		}
//		
//		pos /= (float)selectedObjects.Length;
//		
//		group.transform.position = pos;
//	}

	[MenuItem ("U9/Utils/Flag non zero Z coordinates")]
	public static void FlagNonZeroZ() {
		FlagNonZeroZ (false);
	}

	[MenuItem ("U9/Utils/Force zero Z coordinates")]
	public static void ForceZeroZ() {
		FlagNonZeroZ (true);
	}

	public static void FlagNonZeroZ( bool forceZero ) {
		GameObject[] selectedObjects = Selection.gameObjects;

		Vector3 pos = Vector3.zero;
		foreach( GameObject go in selectedObjects ) {
			FlagNonZeroZ (go.transform, forceZero);
		}
	}

	static void FlagNonZeroZ( Transform t, bool forceZero ) {
		if (t.localPosition.z != 0) {
			Debug.LogError("Non Zero Z: " + t );
			if (forceZero) {
				Vector3 pos = t.localPosition;
				pos.z = 0;
				t.localPosition = pos;
			}
		}
		foreach (Transform child in t) {
			FlagNonZeroZ (child,forceZero);
		}
	}

	[MenuItem ("Assets/Find references to this")]
	public static void FindReferences() {
		Object target = Selection.activeObject;
		Object[] objects = Resources.FindObjectsOfTypeAll (typeof(Object));

		for (int i = 0; i < objects.Length; i++) {

			Object o = objects [i];

			if( o == target ) {
				continue;
			}

			if( i % 100 == 0 ) {
				EditorUtility.DisplayProgressBar("Finding references...",o.name,(float)i/objects.Length);
			}

			try {
				searchedObjects = new List<Object>();
				SearchForTarget( o, target, o.name + "/" );
			} catch( UnityException e ) {
				Debug.LogException(e);
				EditorUtility.ClearProgressBar ();
			}
			
			//			List<Object> referencingObjects = new List<Object> (EditorUtility.CollectDependencies (new Object[] {
//				o
//			}));
//
//			for (int j = 0; j < referencingObjects.Count; j++) {
//				Object o2 = referencingObjects [j];
//				//EditorUtility.DisplayProgressBar("Finding references... (" + i + "/" + objects.Length + ")", o2.name + "(" + j + "/" + referencingObjects.Count + ")",(float)i/objects.Length);
//				if( o2 == target) {
//					Debug.Log ( o + "(" + AssetDatabase.GetAssetOrScenePath(o) + ") is depending on " + target, o );
//
//					searchedObjects = new List<Object>();
//
//					try {
//						SearchForTarget( o, target, "" );
//					} catch( UnityException e ) {
//						Debug.LogException(e);
//						EditorUtility.ClearProgressBar ();
//					}
//				}
//			}

		}

		EditorUtility.ClearProgressBar ();
	}

	static List<Object> searchedObjects;

	static void SearchForTarget( Object searchObject, Object target, string path ) 
	{

		//path += searchObject.name + "(" + searchObject.GetType() + ")/";

		//Debug.Log (path);

		if (searchedObjects.Contains (searchObject)) {
			return;
		}

		searchedObjects.Add (searchObject);

		SerializedObject s = new SerializedObject(searchObject);
		SerializedProperty p = s.GetIterator();
		
		int limiter = 0;

		p.Next (true);
		SerializedProperty endProperty = p.GetEndProperty ();

		do {

			//Debug.Log ("          " + p.name + " - " + p.type );

			if( p.propertyType == SerializedPropertyType.ObjectReference && p.objectReferenceValue != null ) {
				//	Debug.Log ( ">>>>>>>>" + p.depth + " " + p.name + " : " + ( p.objectReferenceValue ? p.objectReferenceValue.name + " - " + p.objectReferenceValue.GetType() : "null" ), p.objectReferenceValue );
				if( p.objectReferenceValue == target ) {
					Debug.Log ("FOUND THE TARGET!!!!: " + path + "(" + p.name + ")", searchObject as GameObject );
				} else if( p.objectReferenceValue != null && p.objectReferenceValue.GetType() == typeof(Texture2D) ) {
				//	Debug.Log ("FOUND A TEXTURE: " + p.objectReferenceValue.name + " AT " + path );
				}else {
					SearchForTarget( p.objectReferenceValue, target, path + p.name + "/" );
				}
			} 

		} while( !SerializedProperty.EqualContents( p, endProperty ) && p.Next (true) );

		if (limiter > 1000) {
			Debug.LogError("REACHED ITERATION LIMIT");
		}
	}

	[MenuItem ("Assets/Find material dependencies")]
	public static void FindMaterialDependencies() {
		Object target = Selection.activeObject;
		List<Object> referencingObjects = new List<Object> (EditorUtility.CollectDependencies (new Object[] {
			target
		}));
		
		for (int j = 0; j < referencingObjects.Count; j++) {
			Object o2 = referencingObjects [j];
			//EditorUtility.DisplayProgressBar("Finding references... (" + j + "/" + referencingObjects.Count + ")", o2.name + "(" + j + "/" + referencingObjects.Count + ")",(float)j/referencingObjects.Count);
			if( o2 is Material ) {
				Debug.Log ( "Material dependency: " + o2 );
				EditorApplication.Beep();
				
				
				
			}
		}
		
		//EditorUtility.ClearProgressBar ();
	}

	[MenuItem ("Assets/Find texture dependencies")]
	public static void FindTextureDependencies() {
		Object target = Selection.activeObject;
		List<Object> referencingObjects = new List<Object> (EditorUtility.CollectDependencies (new Object[] {
			target
			}));
			
			for (int j = 0; j < referencingObjects.Count; j++) {
				Object o2 = referencingObjects [j];
				//EditorUtility.DisplayProgressBar("Finding references... (" + j + "/" + referencingObjects.Count + ")", o2.name + "(" + j + "/" + referencingObjects.Count + ")",(float)j/referencingObjects.Count);
				if( o2 is Texture ) {
					Debug.Log ( "Texture dependency: " + o2 );
					EditorApplication.Beep();



				}
			}

		//EditorUtility.ClearProgressBar ();
	}

	[MenuItem ("Assets/What type is this?")]
	public static void DisplayType() {
		Debug.Log ( Selection.activeObject.GetType() );
	}

	//===============================================================================
	// Utility methods
	//===============================================================================
	
	static public string GetResourcePath(Object obj)
	{
		if( obj != null ) {
			string path = AssetDatabase.GetAssetPath(obj);
			path = path.Remove(0, "Assets/Decromancer/Resources/".Length );
			path = System.IO.Path.GetDirectoryName(path) + "/" + System.IO.Path.GetFileNameWithoutExtension(path);
			
			return path;
		} else {
			return "";
		}
	}
	
	static public T LoadResource<T>(string path) where T : Object
	{
		return (T)Resources.Load(path, typeof(T));
	}
	
	
	//===============================================================================
	// Editor Visual Methods
	//===============================================================================
	
	static public void DrawSeparator()
	{
		GUILayout.Space(12f);
		
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = EditorGUIUtility.whiteTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
			GUI.color = Color.white;
		}
	}
	
	public static void ClearLog()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
		
		System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
		MethodInfo method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}
	
	
	
	[MenuItem ("U9/Utils/Destroy All UIDrawCalls")]
	public static void DestroyUIDrawCalls() {
		Object[] objects = Resources.FindObjectsOfTypeAll (typeof(UIDrawCall));
		foreach( Object o in objects ) {
			Debug.Log ("DC?: " + o );
			UIDrawCall dc = o as UIDrawCall;
			if( dc ) {
				Debug.Log ("DESTROYING: " + dc.name );
				MonoBehaviour.DestroyImmediate(dc.gameObject);
			}
		}
	}
	
	[MenuItem ("U9/Utils/Unload All TextureResourceViews")]
	public static void UnloadTextureResourceViews() {
		Object[] objects = Resources.FindObjectsOfTypeAll (typeof(TextureResourceView));
		foreach( Object o in objects ) {
			TextureResourceView r = o as TextureResourceView;
			UITexture t = r.gameObject.GetComponent<UITexture> ();
			t.mainTexture = null;
			t.material = null;
		}
	}

}
