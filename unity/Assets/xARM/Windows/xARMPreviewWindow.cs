#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using xARM;

public class xARMPreviewWindow : EditorWindow {

	#region Fields
	// Ref to itself
	private static xARMPreviewWindow _myWindow;
	private static string myWindowTitle = "xARM Preview";
	private static int toolbarOffset = 17;
	
	// cache of all active ScreenCaps
	private static List<xARMScreenCap> activeScreenCaps = new List<xARMScreenCap>();
	private static xARMScreenCap selectedScreenCap;
	private static bool updateOnce = false;
	private static int postGameViewResizeFramesToGo = 0;
	// limit update interval
	private double lastUpdateTime = 0.0f;
	private static float updateInterval;
	// switch Game View to new default resolution on next update
	private static bool resizeGameViewToNewDefault = false;

	// GUI
	private static bool repaintNextUpdate = false;
	private static Vector2 scrollPos;
	
	// GUI help (error, warning, info) to display
	public static string WarningBoxText = "";
	public static string InfoBoxText = "";
	#endregion
	
	#region Properties
	private static xARMPreviewWindow myWindow{
		get {
			// refresh Ref if it's lost
			if(!_myWindow) _myWindow = GetWindow<xARMPreviewWindow>();
			return _myWindow;
		}
	}
	#endregion
	
	#region Functions
	[MenuItem ("Window/xARM/xARM Preview", false, 119)]
//	[MenuItem ("Window/xARM/xARM Preview")]
    public static void ShowxARMPreviewWindow() {
		// undock msg
		 if(EditorUtility.DisplayDialog ("xARM will now undock the GameView", 
			"Please keep the GameView undocked while working with xARM.", 
			"OK", "Cancel")){
			_myWindow = GetWindow<xARMPreviewWindow>();
			_myWindow.title = myWindowTitle;
			_myWindow.minSize = new Vector2(300, 150);
			
			// undock GameView only on window open (not while switching between Editor and Play mode)
			xARMManager.FloatingGameView (xARMManager.DefaultGameViewResolution);
		}		
    }
	
	#region OnMessage
	void OnEnable (){
		// Init everything
		xARMManager.CreateProxyGO ();

		// ensure correct display after Edit->Pause/Play mode switch
		repaintNextUpdate = true;
	}
	
	void Update(){
#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		// trace Game View position
		xARMManager.SaveGameViewPosition ();
#else
		// do tracing via Proxy.Update()
#endif

		// ensures a xARM Proxy exists
		xARMManager.CreateProxyGO ();

		// finalize SC update
		if(xARMManager.PreviewIsUpdating && !xARMManager.ScreenCapUpdateInProgress){
			xARMManager.PreviewIsUpdating = false;
			xARMManager.FinalizeScreenCapUpdate ();
		}

		// update SC if auto update is enabled or Update1x was clicked
		if(((xARMManager.Config.PreviewAutoUpdateInEditorMode && xARMManager.CurrEditorMode == EditorMode.Edit) || // Edit
		    (xARMManager.Config.PreviewAutoUpdateInPauseMode && xARMManager.CurrEditorMode == EditorMode.Pause) || // Pause
		    (xARMManager.Config.PreviewAutoUpdateInPlayMode && xARMManager.CurrEditorMode == EditorMode.Play) || // Play
		    updateOnce) &&
		   !xARMManager.GalleryIsUpdating && 
		   !xARMManager.ScreenCapUpdateInProgress)
		{
			
			// limit SC updates
			if(!updateOnce && xARMManager.CurrEditorMode == EditorMode.Edit){ // limit auto update in Edit mode
				updateInterval = 1f / xARMManager.Config.PreviewUpdateIntervalLimitEdit;

			} else if(!updateOnce && xARMManager.CurrEditorMode == EditorMode.Play){ // limit auto update in Play mode
				updateInterval = 1f / xARMManager.Config.PreviewUpdateIntervalLimitPlay;

			} else {
				updateInterval = 0f; // do not limit update interval
			}

			if(EditorApplication.timeSinceStartup > lastUpdateTime + updateInterval){ // limit updates per sec
				lastUpdateTime = EditorApplication.timeSinceStartup;
				
				if(selectedScreenCap is xARMScreenCap && xARMManager.ProxyGO && !xARMManager.GalleryIsUpdating){ 
					xARMScreenCap currScreenCap = selectedScreenCap;
					
					// update screencap only if it's outdated (try one time) OR update1x is clicked
					if(xARMManager.IsToUpdate(currScreenCap) || updateOnce){
						xARMManager.PreviewIsUpdating = true;
						xARMManager.UpdateScreenCap (currScreenCap);
						updateOnce = false;
					}
				}
			}
		}

		// ensure a next frame while waiting for one
		if(xARMManager.ScreenCapUpdateInProgress && xARMManager.PreviewIsUpdating){ // while SC update
			xARMManager.EnsureNextFrame ();
		}
		else if(postGameViewResizeFramesToGo > 0){ // after GV's resolution is changed by selection in Preview
			xARMManager.EnsureNextFrame ();
			postGameViewResizeFramesToGo--;
		}

	 	if(activeScreenCaps.Count == 0){
			InfoBoxText = "No ScreenCaps active. Open xARM Options to activate target Aspect Ratios and Resolutions.";
		} else {
			InfoBoxText = "";
		}

		// resize Game View to new default resolution
		if(resizeGameViewToNewDefault){
			resizeGameViewToNewDefault = false;

			xARMManager.ResizeGameViewToDefault ();
			postGameViewResizeFramesToGo = xARMManager.Config.FramesToWait;
		}

		// repaint window
		if(repaintNextUpdate){
			myWindow.Repaint ();
			repaintNextUpdate = false;
		}
	}
	
	
	void OnGUI () { // is only triggered by Repaint after SC update
		// cache list
		activeScreenCaps = xARMManager.ActiveScreenCaps;
		
		DrawControls ();
		DrawPreview ();
	}
	

	void OnDestroy (){ // isn't executed on Editor close
		// cleanup
		xARMManager.RemoveProxyGO ();
	}
	#endregion
	
	#region Draw
	private static void DrawControls (){
		
		// draw toolbar filling the whole window width
		EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, GUILayout.MaxWidth(myWindow.position.width));
		
		ChangeActiveScreenCap(EditorGUILayout.Popup (xARMManager.Config.PreviewSelectedScreenCapIndex, xARMManager.ScreenCapList, EditorStyles.toolbarDropDown, GUILayout.MaxWidth (250)));
		EditorGUILayout.Space ();

		GUILayout.Label ("Update:", EditorStyles.toolbarButton);
		if(GUILayout.Button ("1x", EditorStyles.toolbarButton)){
			updateOnce = true;
		}
		xARMManager.Config.PreviewAutoUpdateInEditorMode = GUILayout.Toggle (xARMManager.Config.PreviewAutoUpdateInEditorMode, "Edit", EditorStyles.toolbarButton);
		xARMManager.Config.PreviewAutoUpdateInPauseMode = GUILayout.Toggle (xARMManager.Config.PreviewAutoUpdateInPauseMode, "Pause", EditorStyles.toolbarButton);
		xARMManager.Config.PreviewAutoUpdateInPlayMode = GUILayout.Toggle (xARMManager.Config.PreviewAutoUpdateInPlayMode, "Play", EditorStyles.toolbarButton);
		EditorGUILayout.Space ();
		
		xARMManager.Config.PreviewOneToOnePixel = GUILayout.Toggle (xARMManager.Config.PreviewOneToOnePixel, "1:1px", EditorStyles.toolbarButton);
		xARMManager.Config.PreviewOneToOnePhysical = GUILayout.Toggle (xARMManager.Config.PreviewOneToOnePhysical, "1:1phy", EditorStyles.toolbarButton);
		EditorGUILayout.Space ();
		GUILayout.FlexibleSpace ();

		// Tools foldout
		GUILayout.Label ("Tools", EditorStyles.toolbarButton);
		switch (EditorGUILayout.Popup (-1, new string[3] {"Export ScreenCap as PNG", "Export all ScreenCaps as PNGs", "Options"}, EditorStyles.toolbarDropDown, GUILayout.Width(15))){
		case 0: // save selected SC
			xARMManager.SaveScreenCapFile (selectedScreenCap);
			break;

		case 1: // save all SCs
			xARMManager.SaveAllScreenCapFiles ();
			break;

		case 2: // open Options window
			xARMOptionsWindow.DisplayWizard ();
			break;
		}
		EditorGUILayout.EndHorizontal ();
		
		if(xARMManager.Config.PreviewOneToOnePhysical && xARMManager.Config.EditorDPI <= 0) InfoBoxText = "DPI value is invalid. Open xARM Options to set Editor DPI.";
		
#if UNITY_3_3 || UNITY_3_4
		if(WarningBoxText != "") GUILayout.Label (WarningBoxText);
		if(InfoBoxText != "") GUILayout.Label (InfoBoxText);
#else
		if(WarningBoxText != "") EditorGUILayout.HelpBox (WarningBoxText, MessageType.Warning, true); // Unity 3.3 Error
		if(InfoBoxText != "") EditorGUILayout.HelpBox (InfoBoxText, MessageType.Info, true); // Unity 3.3 Error
#endif
	}
	
	
	private static void DrawPreview(){
		if(activeScreenCaps.Count > 0){
			
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			
			// center preview
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();

			
			Rect screenCapRect;
			if(xARMManager.Config.PreviewOneToOnePixel){ // 1:1px
				screenCapRect = GUILayoutUtility.GetRect (selectedScreenCap.Resolution.x, selectedScreenCap.Resolution.y, 
					GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
				
			} else if(xARMManager.Config.PreviewOneToOnePhysical){ // 1:1phy.
				int x = Mathf.RoundToInt (selectedScreenCap.Resolution.x * xARMManager.Config.EditorDPI / selectedScreenCap.DPI);
				int y = Mathf.RoundToInt (selectedScreenCap.Resolution.y * xARMManager.Config.EditorDPI / selectedScreenCap.DPI);
				screenCapRect = GUILayoutUtility.GetRect (x, y, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
				
			} else { // scale to fit
				screenCapRect = GUILayoutUtility.GetRect (myWindow.position.width, myWindow.position.height - toolbarOffset);
			}

#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			// standard draw (with Play mode tint)
			GUI.DrawTexture (screenCapRect, selectedScreenCap.Texture, ScaleMode.ScaleToFit);
#else
			// draw ScreenCap in prepared rect (use EditorGUI.DrawTextureTransparent to draw it without Play mode tint)
			EditorGUI.DrawTextureTransparent (screenCapRect, selectedScreenCap.Texture, ScaleMode.ScaleToFit);
#endif

			// center preview
			GUILayout.EndHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();

			EditorGUILayout.EndScrollView ();
		}
	}

	public static void ChangeActiveScreenCap (int screenCapIndex){
		if(screenCapIndex >= activeScreenCaps.Count) screenCapIndex = 0; // reset if active SC list is shorter now
		xARMManager.Config.PreviewSelectedScreenCapIndex = screenCapIndex;

		// repaint window on next update
		repaintNextUpdate = true;

		if(activeScreenCaps.Count > 0){
			xARMScreenCap previousScreenCap = selectedScreenCap;

			selectedScreenCap = activeScreenCaps[screenCapIndex]; // get Ref to selected SC
			xARMManager.DefaultGameViewResolution = selectedScreenCap.Resolution; // set as new default resolution

			// change GameView resolution if activated and selection was changed
			if(xARMManager.Config.GameViewInheritsPreviewSize && !selectedScreenCap.Equals(previousScreenCap)){
				// switch Game View resolution via next Update() (doesn't work correctly inside OnGUI)
				resizeGameViewToNewDefault = true;
			}
		}
	}
	#endregion
	
	#endregion
}
#endif