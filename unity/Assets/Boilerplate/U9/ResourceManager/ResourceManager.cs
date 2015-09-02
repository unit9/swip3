using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ResourceManager : MonoSingleton<ResourceManager> {

	class LoadedResource {
		
		UnityEngine.Object resource;
		
		int numReferences = 0;

		public int NumReferences {
			get {
				return this.numReferences;
			}
			set {
								//Debug.Log (Resource.name + " = " + value);
				numReferences = value;
			}
		}

		public UnityEngine.Object Resource {
			get {
				return this.resource;
			}
			set {
				resource = value;
			}
		}
	}
	
	Dictionary<string,LoadedResource> loadedResources;

	ListDictionary<string,System.Action<Object>> loadQueue;

	void Awake() {
		Instance = this;
		loadedResources = new Dictionary<string, LoadedResource>();
		loadQueue = new ListDictionary<string, System.Action<Object>>();
	}
	
  

    public void AddToLoadQueue( string path, System.Action<Object> loadCompleteDelegate ) {
		//Debug.Log (string.Format ("AddToLoadQueue: {0}", path));
		LoadedResource r;
		List<System.Action<Object>> loadCompleteDelegates;
		if (loadedResources.TryGetValue (path, out r)) {
			r.NumReferences++;
			loadCompleteDelegate (r.Resource);
		} else {

			loadQueue.Add ( path, loadCompleteDelegate );

			// Added the first one, start the coroutine
			if (loadQueue.Count == 1) {
				StartCoroutine (LoadQueueCoroutine ());
			}
		}
    }

	IEnumerator LoadQueueCoroutine() {
		while( loadQueue.Count > 0 ) {
			 
			ListDictionary<string,System.Action<Object>>.Enumerator e = loadQueue.GetEnumerator();
			if( e.MoveNext() ) {

				KeyValuePair<string,List<System.Action<Object>>> kv = e.Current;
					
				Object o = LoadResource( kv.Key, kv.Value.Count );

				foreach( System.Action<Object> loadCompleteDelegate in kv.Value ) {
					loadCompleteDelegate(o);
				}

				yield return null;

				loadQueue.Remove( kv.Key );
			} else {
				Debug.LogError("WTF");
			}

		//	yield return null;
		}
	}

	public Object LoadResource( string path, int numRefs = 1 ) {
		//Debug.Log("LOAD: " + path);
		LoadedResource r;
		if( loadedResources.TryGetValue( path, out r ) ) {
			r.NumReferences++;
			if (!r.Resource) {
				Debug.LogError ("Found a null resource in cache");
			}
			return r.Resource;
		} else {
			Object o = Resources.Load(path);
			if( !o ) {
				Debug.LogWarning("No resource found at: " + path );
			}
			LoadedResource loadedResource = new LoadedResource() { Resource = o, NumReferences = numRefs };
			loadedResources.Add( path, loadedResource );
			return o;
		}

	}
	
	public void UnloadResource( string path, System.Action<Object> loadCompleteDelegate ) {
	//	Debug.Log("UNLOAD: " + path );
		if( loadedResources.ContainsKey(path) ) {
			LoadedResource r = loadedResources[path];
			r.NumReferences--;
			if( r.NumReferences == 0 ) {
				//Debug.Log ("UNLOADING ASSET: " + r.Resource.name);
				Resources.UnloadAsset( r.Resource );
				loadedResources.Remove(path);
			}
		} else {
						//if (loadQueue.ContainsKey (path)) {
							//	Debug.Log ("LOAD QUEUE BEFORE REMOVAL: " + loadQueue [path].Count);
								loadQueue.Remove (path, loadCompleteDelegate);
							//	if (loadQueue.ContainsKey (path)) {
							//			Debug.Log ("LOAD QUEUE AFTER REMOVAL: " + loadQueue [path].Count);
							//	} else {
				//	Debug.Log ("LOAD QUEUE AFTER REMOVAL: 0");
					//			}
					//	}
			//Debug.LogWarning("No record of any loaded resources at path: " + path );
		}
	}

//#if UNITY_EDITOR
//	void OnGUI() {
//		GUILayout.Label ("QUEUE SIZE: " + loadQueue.Count );
//		foreach (KeyValuePair<string,LoadedResource> kv in loadedResources) {
//			GUILayout.Label (kv.Value.Resource.name + " = " + (Profiler.GetRuntimeMemorySize( kv.Value.Resource ) / (1024 * 1024)).ToString("N3") + " MB (" +  + kv.Value.NumReferences + " refs)");
//		}
//	}
//#endif

}
