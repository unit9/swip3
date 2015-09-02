using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class U9ViewStack : MonoBehaviour
{
	
	Stack<U9View> viewStack;
	
	void Awake ()
	{
		viewStack = new Stack<U9View> ();	
	}
	
	public U9Transition GetPushViewTransition (U9View newView, bool hideOldView = true, bool force = false, bool hideAfter = false )
	{
				
		U9View oldView = null;
		
		if (viewStack.Count > 0) {
			oldView = viewStack.Peek ();
		}

		viewStack.Push (newView);
		
		U9Transition hideOldViewTransition = null, displayNewViewTransition = null;
		
		if (oldView) {
			oldView.DisableInteraction ();
			if (hideOldView) {
				hideOldViewTransition = oldView.GetHideTransition (force);
			}
		}
		displayNewViewTransition = newView.GetDisplayTransition (force);

		if (hideAfter) {
			return U9T.S (displayNewViewTransition,hideOldViewTransition);
		} else {
			return U9T.S (hideOldViewTransition, displayNewViewTransition);
		}
	}
	
	public U9Transition GetPopViewTransition ( int popCount = 1, bool force = false, bool displayFirst = false )
	{

		//PrintStack();

		List<U9Transition> popTransitions = new List<U9Transition>();


		while (viewStack.Count > 0 && popCount > 0 ) {
			popTransitions.Add( viewStack.Pop().GetHideTransition(force) );
			popCount--;
		}
		U9View newView = null;
		if (viewStack.Count > 0) {
			newView = viewStack.Peek ();
		}
		
		U9Transition displayNewView = null;
		if (newView ) {
			if (!newView.IsDisplaying) {
				displayNewView = newView.GetDisplayTransition (force);
			} else {
				newView.EnableInteraction ();
			}
		}

	
	
		//PrintStack();
		if (displayFirst) {
			return U9T.S ( displayNewView, U9T.S (popTransitions.ToArray ()));
		} else {
			return U9T.S (U9T.S (popTransitions.ToArray ()), displayNewView);
		}
	}

	public void ClearStack() {
		if( viewStack.Count > 0 ) {
			viewStack.Peek ().Hide ();
			viewStack.Clear ();
		}
	}

	public void PrintStack ()
	{
		foreach (U9View v in viewStack) {
			Debug.Log ("Stack: " + v.name);
		}
	}

//#if UNITY_EDITOR
//	void OnGUI() {
//		if (gameObject.name == "Singleton") {
//			GUI.color = Color.blue;
//			GUILayout.BeginVertical ();
//			GUILayout.Space (200);
//			GUILayout.EndVertical ();
//
//			GUILayout.BeginVertical ();
//			GUILayout.Label (name + ":");
//			foreach (U9View v in viewStack) {
//				GUILayout.Label (v.name);
//			}
//			GUILayout.EndVertical ();
//
//		}
//
//	}
//#endif

}
