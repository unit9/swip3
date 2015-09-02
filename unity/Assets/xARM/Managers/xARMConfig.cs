#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using xARM;


public class xARMConfig {

	#region Fields	
	
#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	private static string settingsFilePath = Application.dataPath + "/../xARMSettings.xml";
#else
	private static string settingsFilePath = Application.dataPath + "/../ProjectSettings/xARMSettings.xml"; // Unity 3.3 Error
#endif
	
	[XmlIgnore]
	public string xARMVersion = "1.04.318";
	
	[XmlIgnore]
	public bool ConfigChanged = false;
	// limit save interval
	private double nextPossibleSaveTime = 0.0f;
	private float saveInterval = 0.5f;

	// foldouts
	private bool foldoutOptionsPreview = true;
	private bool foldoutOptionsGallery = true;
	private bool foldoutOptions = true;
	private bool foldoutIOS = false;
	private bool foldoutAndroid = false;
	private bool foldoutWinPhone8 = false;
	private bool foldoutWindowsRT = false;
	private bool foldoutStandalone = false;
	private bool foldoutCustom = false;

	// xARM Options
	private bool showLandscape = true;
	private bool showPortrait = false;
	private bool showNavigationBar = false;
	private bool useFixedScreenCapSize = false;
	private Vector2 fixedScreenCapSize = new Vector2(160, 160);
	private bool updatePreviewWhileGameViewHasFocus = true;
	private bool gameViewInheritsPreviewSize = true;
	private Vector2 fallbackGameViewSize = new Vector2(480, 320);
	private bool closeGameViewAfterUpdate = false;
	private int framesToWait = 2;
	private int editorDPI = 0;
	private float scaleRatioCM = 0.9f;
	private string exportPath = "";
	private bool autoTraceGameViewPosition = true;
	private Vector2 fixedGameViewPosition = new Vector2(0,44);
	
	// xARM Preview window
	private int previewSelectedScreenCapIndex = 0;
	private bool previewAutoUpdateInEditorMode = false;
	private bool previewAutoUpdateInPauseMode = false;
	private bool previewAutoUpdateInPlayMode = false;
	private bool previewOneToOnePixel = false;
	private bool previewOneToOnePhysical = false;
	private int previewUpdateIntervalLimitEdit = 12;
	private int previewUpdateIntervalLimitPlay = 24;
	
	// xARM Gallery window
	private bool galleryAutoUpdateInEditorMode = false;
	private bool galleryAutoUpdateInPauseMode = false;
	private int galleryScreenCapsPerRow = 1;
	private int galleryUpdateIntervalLimitEdit = 12;

	// GameView position for Edit<->Play restore
	private Vector2 gameViewPosition = new Vector2(0, 40);
	// list of all ScreenCaps
	public List<xARMScreenCap> AvailScreenCaps = new List<xARMScreenCap>(); // no property; sets ConfigChanged
	#endregion
	
	#region Properties
	// every config change marks config as changed (to enable save on change)
	
	#region Foldouts
	public bool FoldoutOptions {
		get {
			return foldoutOptions;
		}
		set {
			MarkConfigAsChanged (foldoutOptions, value);

			foldoutOptions = value;
		}
	}

	public bool FoldoutOptionsPreview {
		get {
			return foldoutOptionsPreview;
		}
		set {
			MarkConfigAsChanged (foldoutOptionsPreview, value);

			foldoutOptionsPreview = value;
		}
	}
	
	public bool FoldoutOptionsGallery {
		get {
			return foldoutOptionsGallery;
		}
		set {
			MarkConfigAsChanged (foldoutOptionsGallery, value);

			foldoutOptionsGallery = value;
		}
	}
	
	public bool FoldoutIOS {
		get {
			return foldoutIOS;
		}
		set {
			MarkConfigAsChanged (foldoutIOS, value);

			foldoutIOS = value;
		}
	}
	
	public bool FoldoutAndroid {
		get {
			return foldoutAndroid;
		}
		set {
			MarkConfigAsChanged (foldoutAndroid, value);

			foldoutAndroid = value;
		}
	}

	public bool FoldoutWinPhone8 {
		get {
			return foldoutWinPhone8;
		}
		set {
			MarkConfigAsChanged (foldoutWinPhone8, value);

			foldoutWinPhone8 = value;
		}
	}

	public bool FoldoutWindowsRT {
		get {
			return foldoutWindowsRT;
		}
		set {
			MarkConfigAsChanged (foldoutWindowsRT, value);

			foldoutWindowsRT = value;
		}
	}
	
	public bool FoldoutStandalone {
		get {
			return foldoutStandalone;
		}
		set {
			MarkConfigAsChanged (foldoutStandalone, value);

			foldoutStandalone = value;
		}
	}
	
	public bool FoldoutCustom {
		get {
			return foldoutCustom;
		}
		set {
			MarkConfigAsChanged (foldoutCustom, value);

			foldoutCustom = value;
		}
	}
	#endregion

	#region Options (global, Preview and Gallery)
	public bool ShowLandscape {
		get {
			return showLandscape;
		}
		set {
			MarkConfigAsChanged (showLandscape, value);

			showLandscape = value;
		}
	}

	public bool ShowPortrait {
		get {
			return showPortrait;
		}
		set {
			MarkConfigAsChanged (showPortrait, value);

			showPortrait = value;
		}
	}

	public bool ShowNavigationBar {
		get {
			return showNavigationBar;
		}
		set {
			MarkConfigAsChanged (showNavigationBar, value);

			showNavigationBar = value;
		}
	}

	public bool UseFixedScreenCapSize {
		get {
			return useFixedScreenCapSize;
		}
		set {
			MarkConfigAsChanged (useFixedScreenCapSize, value);

			useFixedScreenCapSize = value;
		}
	}

	public Vector2 FixedScreenCapSize{
		get{return fixedScreenCapSize;}
		set {
			MarkConfigAsChanged (fixedScreenCapSize, value);

			if(value.x < 100) value.x = 100;
			if(value.y < 100) value.y = 100;
			
			fixedScreenCapSize = value;
		}
	}

	public bool UpdatePreviewWhileGameViewHasFocus {
		get {
			return updatePreviewWhileGameViewHasFocus;
		}
		set {
			MarkConfigAsChanged (updatePreviewWhileGameViewHasFocus, value);

			updatePreviewWhileGameViewHasFocus = value;
		}
	}

	public bool GameViewInheritsPreviewSize {
		get {
			return gameViewInheritsPreviewSize;
		}
		set {
			MarkConfigAsChanged (gameViewInheritsPreviewSize, value);

			gameViewInheritsPreviewSize = value;
		}
	}

	public Vector2 FallbackGameViewSize{
		get{return fallbackGameViewSize;}
		set {
			MarkConfigAsChanged (fallbackGameViewSize, value);

			if(value.x < 100) value.x = 100;
			if(value.y < 100) value.y = 100;
			
			fallbackGameViewSize = value;
		}
	}

	public bool CloseGameViewAfterUpdate {
		get {
			return closeGameViewAfterUpdate;
		}
		set {
			MarkConfigAsChanged (closeGameViewAfterUpdate, value);

			closeGameViewAfterUpdate = value;
		}
	}

	public int FramesToWait {
		get {
			return framesToWait;
		}
		set {
			MarkConfigAsChanged (framesToWait, value);

			if(value >= 0){
				framesToWait = value;
			} else {
				framesToWait = 1;
			}
		}
	}

	public int EditorDPI {
		get {
			return editorDPI;
		}
		set {
			MarkConfigAsChanged (editorDPI, value);

			editorDPI = value;
		}
	}

	public float ScaleRatioCM {
		get {
			return scaleRatioCM;
		}
		set {
			MarkConfigAsChanged (scaleRatioCM, value);

			scaleRatioCM = value;
		}
	}
	
	public float ScaleRatioInch{
		get {return scaleRatioCM / 2.5f;}
	}

	public string ExportPath {
		get {
			return exportPath;
		}
		set {
			MarkConfigAsChanged (exportPath, value);

			exportPath = value;
		}
	}

	public bool AutoTraceGameViewPosition {
		get {
			return autoTraceGameViewPosition;
		}
		set {
			MarkConfigAsChanged (autoTraceGameViewPosition, value);

			autoTraceGameViewPosition = value;
		}
	}

	public Vector2 FixedGameViewPosition {
		get {
			return fixedGameViewPosition;
		}
		set {
			MarkConfigAsChanged (fixedGameViewPosition, value);

			fixedGameViewPosition = value;
		}
	}

	#endregion

	#region Preview window
	public int PreviewSelectedScreenCapIndex {
		get {
			return previewSelectedScreenCapIndex;
		}
		set {
			MarkConfigAsChanged (previewSelectedScreenCapIndex, value);

			previewSelectedScreenCapIndex = value;
		}
	}

	public bool PreviewAutoUpdateInEditorMode {
		get {
			return previewAutoUpdateInEditorMode;
		}
		set {
			MarkConfigAsChanged (previewAutoUpdateInEditorMode, value);

			previewAutoUpdateInEditorMode = value;
		}
	}

	public bool PreviewAutoUpdateInPauseMode {
		get {
			return previewAutoUpdateInPauseMode;
		}
		set {
			MarkConfigAsChanged (previewAutoUpdateInPauseMode, value);

			previewAutoUpdateInPauseMode = value;
		}
	}

	public bool PreviewAutoUpdateInPlayMode {
		get {
			return previewAutoUpdateInPlayMode;
		}
		set {
			MarkConfigAsChanged (previewAutoUpdateInPlayMode, value);

			previewAutoUpdateInPlayMode = value;
		}
	}

	public bool PreviewOneToOnePixel{
		get {return previewOneToOnePixel;}
		set {
			MarkConfigAsChanged (previewOneToOnePixel, value);

			previewOneToOnePixel = value;
			if(value) previewOneToOnePhysical = false;
		}
	}
	public bool PreviewOneToOnePhysical{
		get {return previewOneToOnePhysical;}
		set {
			MarkConfigAsChanged (previewOneToOnePhysical, value);

			previewOneToOnePhysical = value;
			if(value) previewOneToOnePixel = false;
		}
	}

	public int PreviewUpdateIntervalLimitEdit {
		get {
			return previewUpdateIntervalLimitEdit;
		}
		set {
			MarkConfigAsChanged (previewUpdateIntervalLimitEdit, value);
			
			previewUpdateIntervalLimitEdit = value;
		}
	}
	
	public int PreviewUpdateIntervalLimitPlay {
		get {
			return previewUpdateIntervalLimitPlay;
		}
		set {
			MarkConfigAsChanged (previewUpdateIntervalLimitPlay, value);
			
			previewUpdateIntervalLimitPlay = value;
		}
	}
	#endregion
	
	#region Gallery window
	public bool GalleryAutoUpdateInEditorMode {
		get {
			return galleryAutoUpdateInEditorMode;
		}
		set {
			MarkConfigAsChanged (galleryAutoUpdateInEditorMode, value);

			galleryAutoUpdateInEditorMode = value;
		}
	}

	public bool GalleryAutoUpdateInPauseMode {
		get {
			return galleryAutoUpdateInPauseMode;
		}
		set {
			MarkConfigAsChanged (galleryAutoUpdateInPauseMode, value);

			galleryAutoUpdateInPauseMode = value;
		}
	}
	
	public int GalleryScreenCapsPerRow {
		get {
			return galleryScreenCapsPerRow;
		}
		set {
			MarkConfigAsChanged (galleryScreenCapsPerRow, value);

			galleryScreenCapsPerRow = value;
		}
	}
	
	public int GalleryUpdateIntervalLimitEdit {
		get {
			return galleryUpdateIntervalLimitEdit;
		}
		set {
			MarkConfigAsChanged (galleryUpdateIntervalLimitEdit, value);
			
			galleryUpdateIntervalLimitEdit = value;
		}
	}
	#endregion

	#region div
	public Vector2 GameViewPosition {
		get {
			return gameViewPosition;
		}
		set {
			MarkConfigAsChanged (gameViewPosition, value);

			gameViewPosition = value;
		}
	}

	#endregion

	#endregion
	
	#region Functions
	#region Save
	public xARMConfig() {
		// add save delegate
		EditorApplication.update += SaveOnChange;
	}

	~xARMConfig(){
		// remove save delegate
		EditorApplication.update -= SaveOnChange;
	}

	// save if config has changed
	public void SaveOnChange(){
		// don't save every frame
		if(EditorApplication.timeSinceStartup > nextPossibleSaveTime){
			nextPossibleSaveTime = EditorApplication.timeSinceStartup + saveInterval;

			// save on config change
			if(ConfigChanged || xARMScreenCap.ListChanged){
				Save ();
				
				ConfigChanged = false;
				xARMScreenCap.ListChanged = false;
			}
		}
	}

	// was the value realy changed? (needed because GUI sets value while only displaying it)
	private void MarkConfigAsChanged<T>(T oldValue, T newValue){
		// was the value really changed? (needed because GUI sets value while only displaying it)
		if(!EqualityComparer<T>.Default.Equals (oldValue, newValue)){
			ConfigChanged = true;
		}
	}
	#endregion

	#region Init, Save and Load
	public static xARMConfig InitOrLoad() {
		// load values form XML if possible
		if(File.Exists (settingsFilePath)){
			return Load ();
			
		} else { // default values
			return new xARMConfig();
		}
	}

	// store config to XML
	private void Save() {
		XmlSerializer serializer = new XmlSerializer(typeof(xARMConfig));
		using(StreamWriter stream = new StreamWriter(settingsFilePath, false, Encoding.UTF8)){
			serializer.Serialize (stream, this);
		}
	}
	
	
	// load config from XML
	private static xARMConfig Load() {
		XmlSerializer serializer = new XmlSerializer(typeof(xARMConfig));
		object config;
		xARMConfig loadedxARMConfig;

		// works with legacy (<= v1.03.226) and new (UTF8) files
		using(FileStream stream = new FileStream(settingsFilePath, FileMode.Open)){
			config = serializer.Deserialize (stream); // create xARMConfig instance
		}

		// convert loaded XML
		loadedxARMConfig = (xARMConfig)config;

		// mark loaded config as unchanged to prevent resave
		loadedxARMConfig.ConfigChanged = false;

		return loadedxARMConfig;
	}
	#endregion
	#endregion
}
#endif