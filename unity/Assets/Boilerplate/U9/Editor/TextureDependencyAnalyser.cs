using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class TextureDependencyAnalyser : EditorWindow {

	// Add menu item named "My Window" to the Window menu
	[MenuItem("U9/Utils/Texture Dependency Analyser")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(TextureDependencyAnalyser));
	}

	[System.Serializable]
	class TextureAnalysis {

		public TextureAnalysis( Texture t ) {
			Texture = t;
			Path = AssetDatabase.GetAssetPath(t);
			Megabytes = (float)Profiler.GetRuntimeMemorySize(t)/(1024*1024);
		}

		[SerializeField]
		Texture texture;

		public Texture Texture {
			get {
				return texture;
			}
			set {
				texture = value;
			}
		}

		[SerializeField]
		string path;

		public string Path {
			get {
				return path;
			}
			private set {
				path = value;
			}
		}

		public bool IsProjectAsset {
			get {
				return !string.IsNullOrEmpty(Path);
			}
		}

		[SerializeField]
		float megabytes;

		public float Megabytes {
			get {
				return megabytes;
			}
			private set {
				megabytes = value;
			}
		}

		[SerializeField]
		bool toggleDependentObjects;

		public bool ToggleDependentObjects {
			get {
				return toggleDependentObjects;
			}
			set {
				if( toggleDependentObjects != value ) {
					toggleDependentObjects = value;
					if( toggleDependentObjects ) {
						FindDependentObjects();
					} else {
						dependentObjects.Clear();
					}
				}
			}
		}

		[SerializeField]
		List<Object> dependentObjects = new List<Object>();

		public List<Object> DependentObjects {
			get {
				return dependentObjects;
			}
		}

		void FindDependentObjects() {

//			List<Material> materialsReferencingTexture = new List<Material> ();
//
//			Material[] materials = (Material[])Resources.FindObjectsOfTypeAll (typeof(Material));
//			foreach (Material m in materials) {
//				if( m.mainTexture == Texture ) {
//					materialsReferencingTexture.Add(m);
//				}
//			}



			dependentObjects.Clear ();

			Object[] allGameObjects = Resources.FindObjectsOfTypeAll<Object> ();
			for (int i = 0, ni = allGameObjects.Length; i < ni ; i++) {

				Object o = allGameObjects [i];

				if( i % 50 == 0 && EditorUtility.DisplayCancelableProgressBar( "Searching for dependent objects...", o.name + "(" + i + "/" + ni + ")", (float)i/ni ) ) {
					EditorUtility.ClearProgressBar();
					return;
				}

				List<Object> dependencies = new List<Object> (EditorUtility.CollectDependencies (new Object[] {
					o
				}));

				if( dependencies.Contains(Texture) ) {
					dependentObjects.Add (o);
				}

			}

			EditorUtility.ClearProgressBar();

		}
	}

	[SerializeField]
	List<TextureAnalysis> analysedTextures = new List<TextureAnalysis>();

	[SerializeField]
	Object dependencyRoot;

	Vector2 scrollPos = Vector2.zero;

	void OnGUI() {

		dependencyRoot = EditorGUILayout.ObjectField ("Dependency root: ", dependencyRoot, typeof(Object), false );

		GUILayout.BeginHorizontal ();
		if( GUILayout.Button("Clear") ) {
			analysedTextures.Clear();
		}
		if( GUILayout.Button("Find texture dependencies") ) {
			FindTextureDependencies();
		}
		if( GUILayout.Button("Filter out displayed textures") ) {
			FilterOutDisplayedTextures();
		}
		GUILayout.EndHorizontal ();

		scrollPos = GUILayout.BeginScrollView (scrollPos);
		GUILayout.BeginVertical ();
		foreach (TextureAnalysis ta in analysedTextures) {
			GUILayout.BeginHorizontal ();
			GUILayout.Box( ta.Texture, GUILayout.Width(50f), GUILayout.Height(50f) );
			GUILayout.BeginVertical ();
			GUILayout.Label( ta.Path );
			GUILayout.Label( "Size: " + ta.Megabytes.ToString("G2") + "MB" );
			ta.ToggleDependentObjects = EditorGUILayout.Foldout( ta.ToggleDependentObjects, "Dependent objects:" );
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			if( ta.ToggleDependentObjects ) {

				GUILayout.BeginVertical ();
				foreach( Object o in ta.DependentObjects ) {
					if( o ) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label( o.name + " (" + o.GetType() + ")" );
						if( GUILayout.Button( ">" ) ) {
							EditorGUIUtility.PingObject( o );
						}
						GUILayout.EndHorizontal ();
					}
				}
				GUILayout.EndVertical ();
			}
		}
		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
	}

	void FindLoadedTextures() {
		Texture[] textures = (Texture[])Resources.FindObjectsOfTypeAll (typeof(Texture));
		foreach (Texture t in textures) {
			TextureAnalysis ta = new TextureAnalysis(t);
			if( ta.IsProjectAsset ) {
				analysedTextures.Add ( ta );
			}
		}
	}

	void FindTextureDependencies() {
		List<Object> referencingObjects = new List<Object> (EditorUtility.CollectDependencies (new Object[] {
			dependencyRoot
		}));
		
		for (int j = 0; j < referencingObjects.Count; j++) {
			Object o = referencingObjects [j];
			Texture t = o as Texture;
				if( t ) {
				TextureAnalysis ta = new TextureAnalysis(t);
				if( ta.IsProjectAsset ) {
					analysedTextures.Add ( ta );
				}
			}
		}
	}

	void FilterOutDisplayedTextures() {

		List<Texture> displayedTextures = new List<Texture> ();

		Renderer[] renderers = (Renderer[])Resources.FindObjectsOfTypeAll (typeof(Renderer));
		foreach (Renderer r in renderers) {
			Material[] materials = r.sharedMaterials;
			foreach( Material m in materials ) {
				if( m && m.mainTexture ) {
					displayedTextures.Add(m.mainTexture);
				}
			}
		}

		foreach (Texture t in displayedTextures) {
			TextureAnalysis ta = FindTextureAnalysis(t);
			analysedTextures.Remove(ta);
		}
	}

	TextureAnalysis FindTextureAnalysis( Texture t ) {
		foreach (TextureAnalysis ta in analysedTextures) {
			if( ta.Texture == t ) {
				return ta;
			}
		}
		return null;
	}
}
