#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class xARMDelegatesExample : MonoBehaviour {
	/* This is an example of how you can use the xARM delegates to hook you own code. 
	 * Drop this script on a GameObject in your scene to see the different Screen 
	 * resolutions while updating the ScreenCaps.
	 * 
	 * Note: Do NOT put your custom code into the xARM folder to enable easy updating of xARM.
	 * 
	 * xARMManager.OnPreScreenCapUpdate
	 * 	Use this to run your code directly before a ScreenCap is updated 
	 * 	(e.g. recreate your GUI if it doesn't automatically).
	 * 	The ScreenCap's resolution is set before this is called.
	 * 
	 * xARMManager.OnPostScreenCapUpdate
	 * 	Use this to run your code directly after a ScreenCap is updated 
	 * 	(e.g. reset changes made in OnPreScreenCapUpdate). 
	 * 	The ScreenCap's resolution is still active when this is called.
	 * 
	 * xARMManager.OnFinalizeScreenCapUpdate
	 * 	Use this to run your code directly after all ScreenCaps are updated
	 * 	(e.g. reset GUI or changes made in OnPreScreenCapUpdate).
	 * 	The GameView's default resolution (set in Options) is set before this is called.
	 * 
	 */
	
	// hook delegates
	void OnEnable () {
		xARMManager.OnPreScreenCapUpdate += PreUpdate;
		xARMManager.OnPostScreenCapUpdate += PostUpdate;
		xARMManager.OnFinalizeScreenCapUpdate += FinUpdate;
	}
	
	// unhook delegates
	void OnDisable () {
		xARMManager.OnPreScreenCapUpdate -= PreUpdate;
		xARMManager.OnPostScreenCapUpdate -= PostUpdate;
		xARMManager.OnFinalizeScreenCapUpdate -= FinUpdate;
	}
	
	
	// your custom functions
	private void PreUpdate (){
		Debug.Log ("PreUpdate: " + Screen.width + "x" + Screen.height);
	}
	
	private void PostUpdate (){
		Debug.Log ("PostUpdate: " + Screen.width + "x" + Screen.height);
	}
	
	private void FinUpdate (){
		Debug.Log ("FinUpdate: " + Screen.width + "x" + Screen.height);
	}
	
}
#endif