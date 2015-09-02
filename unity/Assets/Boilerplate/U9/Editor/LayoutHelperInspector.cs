using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LayoutHelper))]
public class LayoutHelperInspector : Editor {

	LayoutHelper layoutHelper;

	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI ();

				EditorGUIUtility.LookLikeInspector ();

		layoutHelper = target as LayoutHelper;

				EditorGUILayout.PrefixLabel ("Transforms:");
				layoutHelper.Transforms = ArrayEditor.DisplayArrayEditor<Transform> (layoutHelper.Transforms, EditTransform);

				DrawSeparator ();

		EditorGUILayout.PrefixLabel ("Layouts:");
		layoutHelper.Layouts = ArrayEditor.DisplayArrayEditor<LayoutHelper.Layout> (layoutHelper.Layouts, EditLayout);

	}

	LayoutHelper.Layout EditLayout( LayoutHelper.Layout layout ) {
				if ( layout == null ) {
			layout = new LayoutHelper.Layout () { LayoutName = "Layout" + layoutHelper.Layouts.Length };
				}

				layout.LayoutName = EditorGUILayout.TextField ( "Name: ", layout.LayoutName );
			
				EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Capture")) {
						layout.CaptureLayout (layoutHelper.Transforms);
		}

		if (GUILayout.Button ("Enforce")) {
			layout.EnforceLayout (layoutHelper.Transforms);
		}

				EditorGUILayout.EndHorizontal ();

				return layout;
	}

	Transform EditTransform( Transform t ) {
		if (!t) {
				return Selection.activeTransform;
		}
				EditorGUILayout.ObjectField (t, typeof(Transform), true);
				return t;
	}

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

}
