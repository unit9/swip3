using UnityEngine;
using System.Collections.Generic;
using System;

public class DetectLeaks : MonoBehaviour {
	
	UnityEngine.Object[] all;
	Dictionary<Type,int> breakdownObjects;
	Dictionary<Type,int> baseLineSizes;
	
	List<Mesh> meshes;
	
	ListDictionary<Type,UnityEngine.Object> objectDictionary;
	
	void Start() {
		DontDestroyOnLoad(gameObject);
		breakdownObjects = new Dictionary<Type, int>();
		objectDictionary = new ListDictionary<System.Type, UnityEngine.Object>();
		baseLineSizes = new Dictionary<Type, int>();
		meshes = new List<Mesh>();
		Resources.UnloadUnusedAssets();
		InvokeRepeating("Detect",3f,3f);
	}
	
	Vector2 scrollPos = Vector2.zero;
	
	int pingIndex = 0;
	Type pingType = null;
	
	bool calculateSize = true;
	
	void Update() {
		if( Input.GetKey( KeyCode.A ) ) {
			MarkBaselineMeshes();
		}
		if( Input.GetKey( KeyCode.S ) ) {
			PrintLeakedMeshes();
		}
		if( Input.GetKey( KeyCode.D ) ) {
			PrintTextures();
		}
	}
	
    void OnGUI () {
		//return;
		
		GUI.color = Color.white;
		
		float totalMemory = 0f;
		
       if( all != null ) {

			//GUILayout.

			calculateSize = GUILayout.Toggle( calculateSize, "Calc Size" );
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			if( GUILayout.Button("Mark as base") ) {
				MarkAsBase();
			}
			if( GUILayout.Button("Mark meshes") ) {
				MarkBaselineMeshes();
			}
			if( GUILayout.Button("Print leaked meshes") ) {
				PrintLeakedMeshes();
			}
			GUILayout.Label("All " + all.Length);
			foreach( KeyValuePair<Type,int> kv in breakdownObjects ) {
				int bytes = kv.Value;
				if( calculateSize && baseLineSizes.ContainsKey(kv.Key) ) {
					bytes -= baseLineSizes[kv.Key];
				}
				float mem = (float)bytes/(1024*1024);
				totalMemory += mem;
				if( kv.Value > 0 ) {
					GUILayout.BeginHorizontal();
					
					if( calculateSize ) {
						GUILayout.Label("\t" + kv.Key + " : " + mem.ToString("N6") + "MB" );
					} else {
						GUILayout.Label("\t" + kv.Key + " : " + kv.Value );
					}
					if( GUILayout.Button("Ping") ) {
						Ping (kv.Key);
					}
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
		}
		float gcMem = (float)System.GC.GetTotalMemory(false)/(1024*1024);
		GUILayout.Label("System.GC.GetTotalMemory: " + gcMem.ToString("N2") );
		totalMemory += gcMem;
		GUILayout.Label("Total memory: " + totalMemory.ToString("N2") + "MB" );
		
    }
	
	void Ping( Type t ) {
		if( pingType != t ) {
			pingIndex = 0;
			pingType = t;
		}
		UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(t);
		//UnityEngine.Object o = objects[pingIndex++];

		foreach( UnityEngine.Object o in objects ) {
			Debug.Log ( o.name + " : " + o.ToString() + ", size: " + ((float)Profiler.GetRuntimeMemorySize(o)/(1024*1024)) + "MB", o );
		}
	}
	
	void Detect() {
		all = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
		breakdownObjects.Clear();
		meshes.Clear();
		foreach( UnityEngine.Object o in all ) {
			objectDictionary.Add(o.GetType(),o);
			if( breakdownObjects.ContainsKey(o.GetType()) ) {					
				breakdownObjects[o.GetType()] += ( calculateSize ? Profiler.GetRuntimeMemorySize(o) : 1 );
			} else {
				breakdownObjects.Add(o.GetType(), ( calculateSize ? Profiler.GetRuntimeMemorySize(o) : 1 ) );
			}
		}
		
		//float gcMem = (float)System.GC.GetTotalMemory(true)/(1024*1024);
		//Debug.Log("System.GC.GetTotalMemory: " + gcMem.ToString("N2") );
	}
	
	void MarkAsBase() {
		baseLineSizes.Clear();
		foreach( KeyValuePair<Type,int> kv in breakdownObjects ) {
			baseLineSizes.Add(kv.Key,kv.Value);
		}
	}
	
	List<Mesh> baselineMeshes;
	void MarkBaselineMeshes() {
		baselineMeshes = new List<Mesh>();
		foreach( UnityEngine.Object o in all ) {
			Mesh m = o as Mesh;
			if( m != null ) {
				baselineMeshes.Add(m);
			}
		}
	}
	
	void PrintLeakedMeshes() {
		foreach( UnityEngine.Object o in all ) {
			Mesh m = o as Mesh;
			if( m != null && !baselineMeshes.Contains(m) ) {
				Debug.Log ("Leaked: " + m.name + " : " + ((float)Profiler.GetRuntimeMemorySize(o)/(1024*1024)) );
			}
		}
	}
	
	void PrintTextures() {
		
		Texture2D[] textures = (Texture2D[])Resources.FindObjectsOfTypeAll(typeof(Texture2D));
		int totalMemory = 0;
		foreach( Texture2D t in textures ) {
			totalMemory += Profiler.GetRuntimeMemorySize(t);
			//Debug.Log ("Tex: (" + t.width + ", " + t.height + ") : " + P );
		}
		Debug.Log ("TOTAL TEX MEM: " + ((float)totalMemory / (1024 * 1024)) + " MB");
	}
	
}
