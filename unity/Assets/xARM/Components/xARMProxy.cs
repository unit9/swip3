using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using xARM;
#endif


[ExecuteInEditMode]
public class xARMProxy : MonoBehaviour {
	#if UNITY_EDITOR
	
	#region Fields
	private static Camera _myCamera;
	#endregion
	
	#region Properties
	private Camera myCamera{
		get {
			if(!_myCamera){
				_myCamera = camera;
			}
			return _myCamera;
		}
	}
	#endregion
	
	#region Function
	void Start(){
#if UNITY_3_3 || UNITY_3_4
		// not supported
#else
		// add delegate to reset on scene switch
		EditorApplication.hierarchyWindowChanged += xARMManager.ResetOnSceneSwitch;
#endif
	}
	
	void Update(){
		// self destroy if Ref to EditorWindow is lost
		if(xARMManager.Proxy != this) DestroyImmediate (this.gameObject);
		
		// skip updates caused by xARM or GameView resize
		if(xARMManager.ExecuteUpdate){
			xARMManager.SceneChanged (); // mark scene as changed
		}

#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		// do tracing via EditorWindow.Update() (changing GV position doesn't trigger Proxy.Update())
#else
		// trace Game View position
		xARMManager.SaveGameViewPosition ();
#endif
	}

	#region Update SC
	public void StartUpdateScreenCapCoroutine(xARMScreenCap screenCapToUpdate){
		StartCoroutine (UpdateScreenCapCoroutine (screenCapToUpdate));
	}
	
	// coroutine to update the ScreenCap at the end of the current frame
	private IEnumerator UpdateScreenCapCoroutine(xARMScreenCap screenCapToUpdate){
		yield return new WaitForEndOfFrame();
		xARMManager.ReadScreenCapFromGameView (screenCapToUpdate);
		xARMManager.ScreenCapUpdateInProgress = false;
	}
	#endregion
	
	#region Wait x frames
	// coroutine to wait a few frames between resolution change and SC update
	public void StartWaitXFramesCoroutine(xARMScreenCap screenCap, int frameCount){
		StartCoroutine (WaitXFrames (screenCap, frameCount));
	}
	
	private IEnumerator WaitXFrames(xARMScreenCap screenCap, int frameCount){
		while (frameCount > 0){
			yield return null; // wait until next frame
			
			frameCount--;
		}
		xARMManager.UpdateScreenCapAtEOF (screenCap);
	}
	#endregion
	#endregion
	
	#endif
}