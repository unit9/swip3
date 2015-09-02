using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Collections;
using EveryplayMiniJSON;

public class Everyplay : MonoBehaviour
{
    // Enumerations

    public enum FaceCamPreviewOrigin
    {
        TopLeft = 0,
        TopRight,
        BottomLeft,
        BottomRight
    };

    public enum UserInterfaceIdiom
    {
        Phone = 0,
        Tablet,
        iPhone = Phone,
        iPad = Tablet
    };

    // Delegates and events

    public delegate void WasClosedDelegate();

    public static event WasClosedDelegate WasClosed;

    public delegate void ReadyForRecordingDelegate(bool enabled);

    public static event ReadyForRecordingDelegate ReadyForRecording;

    public delegate void RecordingStartedDelegate();

    public static event RecordingStartedDelegate RecordingStarted;

    public delegate void RecordingStoppedDelegate();

    public static event RecordingStoppedDelegate RecordingStopped;

    public delegate void FaceCamSessionStartedDelegate();

    public static event FaceCamSessionStartedDelegate FaceCamSessionStarted;

    public delegate void FaceCamRecordingPermissionDelegate(bool granted);

    public static event FaceCamRecordingPermissionDelegate FaceCamRecordingPermission;

    public delegate void FaceCamSessionStoppedDelegate();

    public static event FaceCamSessionStoppedDelegate FaceCamSessionStopped;

    public delegate void ThumbnailReadyAtFilePathDelegate(string filePath);

    public static event ThumbnailReadyAtFilePathDelegate ThumbnailReadyAtFilePath;

    public delegate void ThumbnailReadyAtTextureIdDelegate(int textureId,bool portrait);

    public static event ThumbnailReadyAtTextureIdDelegate ThumbnailReadyAtTextureId;

    public delegate void UploadDidStartDelegate(int videoId);

    public static event UploadDidStartDelegate UploadDidStart;

    public delegate void UploadDidProgressDelegate(int videoId,float progress);

    public static event UploadDidProgressDelegate UploadDidProgress;

    public delegate void UploadDidCompleteDelegate(int videoId);

    public static event UploadDidCompleteDelegate UploadDidComplete;

    public delegate void ThumbnailLoadReadyDelegate(Texture2D texture);

    public delegate void ThumbnailLoadFailedDelegate(string error);

    public delegate void RequestReadyDelegate(string response);

    public delegate void RequestFailedDelegate(string error);

    // Private member variables

    private static string clientId;
    private static bool appIsClosing = false;

    // For some time we want to support calling Everyplay with the old SharedInstance.
    // This requires us to use a EveryplayLegacy instance wrapper.
    // We can deprecate SharedInstance and notify the user when
    // using the old way. After some time we can remove it totally.
    [Obsolete("Calling Everyplay with SharedInstance is deprecated, you may remove SharedInstance.")]
    public static EveryplayLegacy SharedInstance {
        get {
            // Reference to EveryplayInstance to make sure the real instance exists, also create a legacy wrapper
            if(EveryplayInstance != null) {
                if(everyplayLegacy == null) {
                    // Add legacy wrapper only when SharedInstance is referenced
                    everyplayLegacy = everyplayInstance.gameObject.AddComponent<EveryplayLegacy>();
                }
            }
            return everyplayLegacy;
        }
    }

    private static EveryplayLegacy everyplayLegacy = null;

    // The real singleton, SharedInstance is for legacy support only
    private static Everyplay everyplayInstance = null;

    private static Everyplay EveryplayInstance {
        get {
            if(everyplayInstance == null && !appIsClosing) {
                EveryplaySettings settings = (EveryplaySettings)Resources.Load("EveryplaySettings");

                if(settings != null) {
                    if(settings.IsEnabled) {
                        GameObject everyplayGameObject = new GameObject("Everyplay");

                        if(everyplayGameObject != null) {
                            everyplayInstance = everyplayGameObject.AddComponent<Everyplay>();

                            if(everyplayInstance != null) {
                                clientId = settings.clientId;

                                // Initialize the native
                                #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
                                InitEveryplay(settings.clientId, settings.clientSecret, settings.redirectURI);
                                #endif

                                // Add test buttons if requested
                                if(settings.testButtonsEnabled) {
                                   AddTestButtons(everyplayGameObject);
                                }

                                DontDestroyOnLoad(everyplayGameObject);
                            }
                        }
                    }
                }
            }

            return everyplayInstance;
        }
    }

    // Public static methods

    public static void Initialize()
    {
        // If everyplayInstance is not yet initialized, calling EveryplayInstance property getter will trigger the initialization
        if(EveryplayInstance == null) {
            Debug.Log("Unable to initialize Everyplay. Everyplay might be disabled for this platform or the app is closing.");
        }
    }

    public static void Show()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayShow();
            #endif
        }
    }

    public static void ShowWithPath(string path)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayShowWithPath(path);
            #endif
        }
    }

    public static void PlayVideoWithURL(string url)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayPlayVideoWithURL(url);
            #endif
        }
    }

    public static void PlayVideoWithDictionary(Dictionary<string,object> dict)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayPlayVideoWithDictionary(Json.Serialize(dict));
            #endif
        }
    }

    public static void MakeRequest(string method, string url, Dictionary<string, object> data, Everyplay.RequestReadyDelegate readyDelegate, Everyplay.RequestFailedDelegate failedDelegate)
    {
        if(EveryplayInstance != null) {
            EveryplayInstance.AsyncMakeRequest(method, url, data, readyDelegate, failedDelegate);
        }
    }

    public static string AccessToken()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayAccountAccessToken();
            #endif
        }
        return null;
    }

    public static void ShowSharingModal()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayShowSharingModal();
            #endif
        }
    }

    public static void StartRecording()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayStartRecording();
            #endif
        }
    }

    public static void StopRecording()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayStopRecording();
            #endif
        }
    }

    public static void PauseRecording()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayPauseRecording();
            #endif
        }
    }

    public static void ResumeRecording()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayResumeRecording();
            #endif
        }
    }

    public static bool IsRecording()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayIsRecording();
            #endif
        }
        return false;
    }

    public static bool IsRecordingSupported()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayIsRecordingSupported();
            #endif
        }
        return false;
    }

    public static bool IsPaused()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayIsPaused();
            #endif
        }
        return false;
    }

    public static bool SnapshotRenderbuffer()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplaySnapshotRenderbuffer();
            #endif
        }
        return false;
    }

    public static bool IsSupported()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayIsSupported();
            #endif
        }
        return false;
    }

    public static bool IsSingleCoreDevice()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayIsSingleCoreDevice();
            #endif
        }
        return false;
    }

    public static int GetUserInterfaceIdiom()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayGetUserInterfaceIdiom();
            #endif
        }
        return 0;
    }

    public static void PlayLastRecording()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayPlayLastRecording();
            #endif
        }
    }

    public static void SetMetadata(string key, object val)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            if(key != null && val != null) {
                Dictionary<string,object> dict = new Dictionary<string, object>();
                dict.Add(key, val);
                EveryplaySetMetadata(Json.Serialize(dict));
            }
            #endif
        }
    }

    public static void SetMetadata(Dictionary<string,object> dict)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            if(dict != null) {
                if(dict.Count > 0) {
                    EveryplaySetMetadata(Json.Serialize(dict));
                }
            }
            #endif
        }
    }

    public static void SetTargetFPS(int fps)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetTargetFPS(fps);
            #endif
        }
    }

    public static void SetMotionFactor(int factor)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetMotionFactor(factor);
            #endif
        }
    }

    public static void SetMaxRecordingMinutesLength(int minutes)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetMaxRecordingMinutesLength(minutes);
            #endif
        }
    }

    public static void SetLowMemoryDevice(bool state)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetLowMemoryDevice(state);
            #endif
        }
    }

    public static void SetDisableSingleCoreDevices(bool state)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetDisableSingleCoreDevices(state);
            #endif
        }
    }

    public static void LoadThumbnailFromFilePath(string filePath, Everyplay.ThumbnailLoadReadyDelegate readyDelegate, Everyplay.ThumbnailLoadFailedDelegate failedDelegate)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayInstance.AsyncLoadThumbnailFromFilePath(filePath, readyDelegate, failedDelegate);
            #endif
        }
    }

    public static bool FaceCamIsVideoRecordingSupported()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayFaceCamIsVideoRecordingSupported();
            #endif
        }
        return false;
    }

    public static bool FaceCamIsAudioRecordingSupported()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayFaceCamIsAudioRecordingSupported();
            #endif
        }
        return false;
    }

    public static bool FaceCamIsHeadphonesPluggedIn()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayFaceCamIsHeadphonesPluggedIn();
            #endif
        }
        return false;
    }

    public static bool FaceCamIsSessionRunning()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayFaceCamIsSessionRunning();
            #endif
        }
        return false;
    }

    public static bool FaceCamIsRecordingPermissionGranted()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayFaceCamIsRecordingPermissionGranted();
            #endif
        }
        return false;
    }

    public static float FaceCamAudioPeakLevel()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayFaceCamAudioPeakLevel();
            #endif
        }
        return 0.0f;
    }

    public static float FaceCamAudioPowerLevel()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            return EveryplayFaceCamAudioPowerLevel();
            #endif
        }
        return 0.0f;
    }

    public static void FaceCamSetMonitorAudioLevels(bool enabled)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetMonitorAudioLevels(enabled);
            #endif
        }
    }

    public static void FaceCamSetAudioOnly(bool audioOnly)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetAudioOnly(audioOnly);
            #endif
        }
    }

    public static void FaceCamSetPreviewVisible(bool visible)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewVisible(visible);
            #endif
        }
    }

    public static void FaceCamSetPreviewScaleRetina(bool autoScale)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewScaleRetina(autoScale);
            #endif
        }
    }

    public static void FaceCamSetPreviewSideWidth(int width)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewSideWidth(width);
            #endif
        }
    }

    public static void FaceCamSetPreviewBorderWidth(int width)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewBorderWidth(width);
            #endif
        }
    }

    public static void FaceCamSetPreviewPositionX(int x)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewPositionX(x);
            #endif
        }
    }

    public static void FaceCamSetPreviewPositionY(int y)
    {
        if(EveryplayInstance != null) {
         #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewPositionY(y);
         #endif
        }
    }

    public static void FaceCamSetPreviewBorderColor(float r, float g, float b, float a)
    {
        if(EveryplayInstance != null) {
            #if (UNITY_IPHONE && EVERYPLAY_IPHONE) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewBorderColor(r, g, b, a);
            #endif
        }
    }

    public static void FaceCamSetPreviewOrigin(Everyplay.FaceCamPreviewOrigin origin)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetPreviewOrigin((int)origin);
            #endif
        }
    }

    public static void SetThumbnailWidth(int thumbnailWidth)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetThumbnailWidth(thumbnailWidth);
            #endif
        }
    }

    public static void FaceCamSetTargetTextureId(int textureId)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetTargetTextureId(textureId);
            #endif
        }
    }

    public static void FaceCamSetTargetTextureWidth(int textureWidth)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetTargetTextureWidth(textureWidth);
            #endif
        }
    }

    public static void FaceCamSetTargetTextureHeight(int textureHeight)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamSetTargetTextureHeight(textureHeight);
            #endif
        }
    }

    public static void FaceCamStartSession()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamStartSession();
            #endif
        }
    }

    public static void FaceCamRequestRecordingPermission()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamRequestRecordingPermission();
            #endif
        }
    }

    public static void FaceCamStopSession()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayFaceCamStopSession();
            #endif
        }
    }

    public static void SetThumbnailTargetTextureId(int textureId)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetThumbnailTargetTextureId(textureId);
            #endif
        }
    }

    public static void SetThumbnailTargetTextureWidth(int textureWidth)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetThumbnailTargetTextureWidth(textureWidth);
            #endif
        }
    }

    public static void SetThumbnailTargetTextureHeight(int textureHeight)
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplaySetThumbnailTargetTextureHeight(textureHeight);
            #endif
        }
    }

    public static void TakeThumbnail()
    {
        if(EveryplayInstance != null) {
            #if ((UNITY_IPHONE && EVERYPLAY_IPHONE) || (UNITY_ANDROID && EVERYPLAY_ANDROID)) && !UNITY_EDITOR
            EveryplayTakeThumbnail();
            #endif
        }
    }

    // Private static methods

    private static void RemoveAllEventHandlers()
    {
        if(WasClosed != null) {
            foreach(Everyplay.WasClosedDelegate del in WasClosed.GetInvocationList()) {
                WasClosed -= del;
            }
        }

        if(ReadyForRecording != null) {
            foreach(Everyplay.ReadyForRecordingDelegate del in ReadyForRecording.GetInvocationList()) {
                ReadyForRecording -= del;
            }
        }

        if(RecordingStarted != null) {
            foreach(Everyplay.RecordingStartedDelegate del in RecordingStarted.GetInvocationList()) {
                RecordingStarted -= del;
            }
        }

        if(RecordingStopped != null) {
            foreach(Everyplay.RecordingStoppedDelegate del in RecordingStopped.GetInvocationList()) {
                RecordingStopped -= del;
            }
        }

        if(FaceCamSessionStarted != null) {
            foreach(Everyplay.FaceCamSessionStartedDelegate del in FaceCamSessionStarted.GetInvocationList()) {
                FaceCamSessionStarted -= del;
            }
        }

        if(FaceCamRecordingPermission != null) {
            foreach(Everyplay.FaceCamRecordingPermissionDelegate del in FaceCamRecordingPermission.GetInvocationList()) {
                FaceCamRecordingPermission -= del;
            }
        }

        if(FaceCamSessionStopped != null) {
            foreach(Everyplay.FaceCamSessionStoppedDelegate del in FaceCamSessionStopped.GetInvocationList()) {
                FaceCamSessionStopped -= del;
            }
        }

        if(ThumbnailReadyAtFilePath != null) {
            foreach(Everyplay.ThumbnailReadyAtFilePathDelegate del in ThumbnailReadyAtFilePath.GetInvocationList()) {
                ThumbnailReadyAtFilePath -= del;
            }
        }

        if(ThumbnailReadyAtTextureId != null) {
            foreach(Everyplay.ThumbnailReadyAtTextureIdDelegate del in ThumbnailReadyAtTextureId.GetInvocationList()) {
                ThumbnailReadyAtTextureId -= del;
            }
        }

        if(UploadDidStart != null) {
            foreach(Everyplay.UploadDidStartDelegate del in UploadDidStart.GetInvocationList()) {
                UploadDidStart -= del;
            }
        }

        if(UploadDidProgress != null) {
            foreach(Everyplay.UploadDidProgressDelegate del in UploadDidProgress.GetInvocationList()) {
                UploadDidProgress -= del;
            }
        }

        if(UploadDidComplete != null) {
            foreach(Everyplay.UploadDidCompleteDelegate del in UploadDidComplete.GetInvocationList()) {
                UploadDidComplete -= del;
            }
        }
    }

    private static void AddTestButtons(GameObject gameObject) {
        Texture2D textureAtlas = (Texture2D)Resources.Load("everyplay-test-buttons", typeof(Texture2D));
        if(textureAtlas != null) {
            EveryplayRecButtons recButtons = gameObject.AddComponent<EveryplayRecButtons>();
            if(recButtons != null) {
                recButtons.atlasTexture = textureAtlas;
            }
        }
    }

    // Private instance methods

    private void AsyncLoadThumbnailFromFilePath(string filePath, Everyplay.ThumbnailLoadReadyDelegate readyDelegateMethod, Everyplay.ThumbnailLoadFailedDelegate failedDelegateMethod)
    {
        if(filePath != null) {
            StartCoroutine(LoadThumbnailEnumerator(filePath, readyDelegateMethod, failedDelegateMethod));
        }
        else {
            failedDelegateMethod("Everyplay error: Thumbnail is not ready.");
        }
    }

    private IEnumerator LoadThumbnailEnumerator(string fileName, Everyplay.ThumbnailLoadReadyDelegate readyDelegateMethod, Everyplay.ThumbnailLoadFailedDelegate failedDelegateMethod)
    {
        WWW www = new WWW("file://" + fileName);

        yield return www;

        if(!string.IsNullOrEmpty(www.error)) {
            failedDelegateMethod("Everyplay error: " + www.error);
        }
        else {
            if(www.texture) {
                readyDelegateMethod(www.texture);
            }
            else {
                failedDelegateMethod("Everyplay error: Loading thumbnail failed.");
            }
        }
    }

    private void AsyncMakeRequest(string method, string url, Dictionary<string, object> data, Everyplay.RequestReadyDelegate readyDelegate, Everyplay.RequestFailedDelegate failedDelegate)
    {
        StartCoroutine(MakeRequestEnumerator(method, url, data, readyDelegate, failedDelegate));
    }

    private IEnumerator MakeRequestEnumerator(string method, string url, Dictionary<string, object> data, Everyplay.RequestReadyDelegate readyDelegate, Everyplay.RequestFailedDelegate failedDelegate)
    {
        if(data == null) {
            data = new Dictionary<string, object>();
        }

        if(url.IndexOf("http") != 0) {
            if(url.IndexOf("/") != 0) {
                url = "/" + url;
            }

            url = "https://api.everyplay.com" + url;
        }

        method = method.ToLower();

#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
        Hashtable headers = new Hashtable();
#else
        Dictionary<string, string> headers = new Dictionary<string, string>();
#endif

        string accessToken = AccessToken();
        if(accessToken != null) {
            headers["Authorization"] = "Bearer " + accessToken;
        }
        else {
            if(url.IndexOf("client_id") == -1) {
                if(url.IndexOf("?") == -1) {
                    url += "?";
                }
                else {
                    url += "&";
                }
                url += "client_id=" + clientId;
            }
        }

        data.Add("_method", method);

        string dataString = Json.Serialize(data);
        byte[] dataArray = System.Text.Encoding.UTF8.GetBytes(dataString);

        headers["Accept"] = "application/json";
        headers["Content-Type"] = "application/json";
        headers["Data-Type"] = "json";
        headers["Content-Length"] = dataArray.Length.ToString();

        WWW www = new WWW(url, dataArray, headers);

        yield return www;

        if(!string.IsNullOrEmpty(www.error) && failedDelegate != null) {
            failedDelegate("Everyplay error: " + www.error);
        }
        else if(string.IsNullOrEmpty(www.error) && readyDelegate != null) {
            readyDelegate(www.text);
        }
    }

    // Monobehaviour methods

    void OnApplicationQuit()
    {
        RemoveAllEventHandlers();
        appIsClosing = true;
        everyplayInstance = null;
    }

    // Private instance methods called by native

    private void EveryplayHidden(string msg)
    {
        if(WasClosed != null) {
            WasClosed();
        }
    }

    private void EveryplayReadyForRecording(string jsonMsg)
    {
        if(jsonMsg != null && jsonMsg.Length > 0) {
            Dictionary<string,object> dict = Json.Deserialize(jsonMsg) as Dictionary<string,object>;
            if(dict != null && dict.ContainsKey("enabled")) {
                int enabled = Convert.ToInt32(dict["enabled"]);
                if(ReadyForRecording != null) {
                    ReadyForRecording(enabled == 1);
                }
            }
        }
    }

    private void EveryplayRecordingStarted(string msg)
    {
        if(RecordingStarted != null) {
            RecordingStarted();
        }
    }

    private void EveryplayRecordingStopped(string msg)
    {
        if(RecordingStopped != null) {
            RecordingStopped();
        }
    }

    private void EveryplayFaceCamSessionStarted(string msg)
    {
        if(FaceCamSessionStarted != null) {
            FaceCamSessionStarted();
        }
    }

    private void EveryplayFaceCamRecordingPermission(string jsonMsg)
    {
        if(jsonMsg != null && jsonMsg.Length > 0) {
            Dictionary<string,object> dict = Json.Deserialize(jsonMsg) as Dictionary<string,object>;
            if(dict != null && dict.ContainsKey("granted")) {
                int granted = Convert.ToInt32(dict["granted"]);
                if(FaceCamRecordingPermission != null) {
                    FaceCamRecordingPermission(granted == 1);
                }
            }
        }
    }

    private void EveryplayFaceCamSessionStopped(string msg)
    {
        if(FaceCamSessionStopped != null) {
            FaceCamSessionStopped();
        }
    }

    private void EveryplayThumbnailReadyAtFilePath(string jsonMsg)
    {
        if(jsonMsg != null && jsonMsg.Length > 0) {
            Dictionary<string,object> dict = Json.Deserialize(jsonMsg) as Dictionary<string,object>;
            if(dict != null && dict.ContainsKey("thumbnailFilePath")) {
                string realFilePath = (string)dict["thumbnailFilePath"];
                if(realFilePath != null) {
                    if(ThumbnailReadyAtFilePath != null) {
                        ThumbnailReadyAtFilePath(realFilePath);
                    }
                }
            }
        }
    }

    private void EveryplayThumbnailReadyAtTextureId(string jsonMsg)
    {
        if(jsonMsg != null && jsonMsg.Length > 0) {
            Dictionary<string,object> dict = Json.Deserialize(jsonMsg) as Dictionary<string,object>;
            if(dict != null && dict.ContainsKey("textureId") && dict.ContainsKey("portrait")) {
                int realTextureId = Convert.ToInt32(dict["textureId"]);
                bool realPortrait = Convert.ToInt32(dict["portrait"]) > 0 ? true : false;
                if(ThumbnailReadyAtTextureId != null) {
                    ThumbnailReadyAtTextureId(realTextureId, realPortrait);
                }
            }
        }
    }

    private void EveryplayUploadDidStart(string jsonMsg)
    {
        if(jsonMsg != null && jsonMsg.Length > 0) {
            Dictionary<string,object> dict = Json.Deserialize(jsonMsg) as Dictionary<string,object>;
            if(dict != null && dict.ContainsKey("videoId")) {
                int videoId = Convert.ToInt32(dict["videoId"]);
                if(UploadDidStart != null) {
                    UploadDidStart(videoId);
                }
            }
        }
    }

    private void EveryplayUploadDidProgress(string jsonMsg)
    {
        if(jsonMsg != null && jsonMsg.Length > 0) {
            Dictionary<string,object> dict = Json.Deserialize(jsonMsg) as Dictionary<string,object>;
            if(dict != null && dict.ContainsKey("videoId") && dict.ContainsKey("progress")) {
                int videoId = Convert.ToInt32(dict["videoId"]);
                double progress = Convert.ToDouble(dict["progress"]);
                if(UploadDidProgress != null) {
                    UploadDidProgress(videoId, (float)progress);
                }
            }
        }
    }

    private void EveryplayUploadDidComplete(string jsonMsg)
    {
        if(jsonMsg != null && jsonMsg.Length > 0) {
            Dictionary<string,object> dict = Json.Deserialize(jsonMsg) as Dictionary<string,object>;
            if(dict != null && dict.ContainsKey("videoId")) {
                int videoId = Convert.ToInt32(dict["videoId"]);
                if(UploadDidComplete != null) {
                    UploadDidComplete(videoId);
                }
            }
        }
    }

    // Native calls

    #if (UNITY_IPHONE && EVERYPLAY_IPHONE) && !UNITY_EDITOR

    [DllImport("__Internal")]
    public static extern void InitEveryplay(string clientId, string clientSecret, string redirectURI);

    [DllImport("__Internal")]
    private static extern void EveryplayShow();

    [DllImport("__Internal")]
    private static extern void EveryplayShowWithPath(string path);

    [DllImport("__Internal")]
    private static extern void EveryplayPlayVideoWithURL(string url);

    [DllImport("__Internal")]
    private static extern void EveryplayPlayVideoWithDictionary(string dic);

    [DllImport("__Internal")]
    private static extern string EveryplayAccountAccessToken();

    [DllImport("__Internal")]
    private static extern void EveryplayShowSharingModal();

    [DllImport("__Internal")]
    private static extern void EveryplayStartRecording();

    [DllImport("__Internal")]
    private static extern void EveryplayStopRecording();

    [DllImport("__Internal")]
    private static extern void EveryplayPauseRecording();

    [DllImport("__Internal")]
    private static extern void EveryplayResumeRecording();

    [DllImport("__Internal")]
    private static extern bool EveryplayIsRecording();

    [DllImport("__Internal")]
    private static extern bool EveryplayIsRecordingSupported();

    [DllImport("__Internal")]
    private static extern bool EveryplayIsPaused();

    [DllImport("__Internal")]
    private static extern bool EveryplaySnapshotRenderbuffer();

    [DllImport("__Internal")]
    private static extern void EveryplayPlayLastRecording();

    [DllImport("__Internal")]
    private static extern void EveryplaySetMetadata(string json);

    [DllImport("__Internal")]
    private static extern void EveryplaySetTargetFPS(int fps);

    [DllImport("__Internal")]
    private static extern void EveryplaySetMotionFactor(int factor);

    [DllImport("__Internal")]
    private static extern void EveryplaySetMaxRecordingMinutesLength(int minutes);

    [DllImport("__Internal")]
    private static extern void EveryplaySetLowMemoryDevice(bool state);

    [DllImport("__Internal")]
    private static extern void EveryplaySetDisableSingleCoreDevices(bool state);

    [DllImport("__Internal")]
    private static extern bool EveryplayIsSupported();

    [DllImport("__Internal")]
    private static extern bool EveryplayIsSingleCoreDevice();

    [DllImport("__Internal")]
    private static extern int EveryplayGetUserInterfaceIdiom();

    [DllImport("__Internal")]
    private static extern bool EveryplayFaceCamIsVideoRecordingSupported();

    [DllImport("__Internal")]
    private static extern bool EveryplayFaceCamIsAudioRecordingSupported();

    [DllImport("__Internal")]
    private static extern bool EveryplayFaceCamIsHeadphonesPluggedIn();

    [DllImport("__Internal")]
    private static extern bool EveryplayFaceCamIsSessionRunning();

    [DllImport("__Internal")]
    private static extern bool EveryplayFaceCamIsRecordingPermissionGranted();

    [DllImport("__Internal")]
    private static extern float EveryplayFaceCamAudioPeakLevel();

    [DllImport("__Internal")]
    private static extern float EveryplayFaceCamAudioPowerLevel();

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetMonitorAudioLevels(bool enabled);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetAudioOnly(bool audioOnly);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewVisible(bool visible);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewScaleRetina(bool autoScale);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewSideWidth(int width);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewBorderWidth(int width);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewPositionX(int x);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewPositionY(int y);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewBorderColor(float r, float g, float b, float a);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetPreviewOrigin(int origin);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetTargetTextureId(int textureId);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetTargetTextureWidth(int textureWidth);

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamSetTargetTextureHeight(int textureHeight);

    [DllImport("__Internal")]
    private static extern bool EveryplayFaceCamStartSession();

    [DllImport("__Internal")]
    private static extern void EveryplayFaceCamRequestRecordingPermission();

    [DllImport("__Internal")]
    private static extern bool EveryplayFaceCamStopSession();

    [DllImport("__Internal")]
    private static extern void EveryplaySetThumbnailWidth(int thumbnailWidth);

    [DllImport("__Internal")]
    private static extern void EveryplaySetThumbnailTargetTextureId(int textureId);

    [DllImport("__Internal")]
    private static extern void EveryplaySetThumbnailTargetTextureWidth(int textureWidth);

    [DllImport("__Internal")]
    private static extern void EveryplaySetThumbnailTargetTextureHeight(int textureHeight);

    [DllImport("__Internal")]
    private static extern void EveryplayTakeThumbnail();

    #elif (UNITY_ANDROID && EVERYPLAY_ANDROID) && !UNITY_EDITOR

    private static AndroidJavaObject everyplayUnity;

    public static void InitEveryplay(string clientId, string clientSecret, string redirectURI)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        everyplayUnity = new AndroidJavaObject("com.everyplay.Everyplay.unity.EveryplayUnity3DWrapper");
        everyplayUnity.Call("initEveryplay", activity, clientId, clientSecret, redirectURI);
    }

    public static void EveryplayShow()
    {
        everyplayUnity.Call<bool>("showEveryplay");
    }

    public static void EveryplayShowWithPath(string path)
    {
        everyplayUnity.Call<bool>("showEveryplay", path);
    }

    public static void EveryplayPlayVideoWithURL(string url)
    {
        everyplayUnity.Call("playVideoWithURL", url);
    }

    public static void EveryplayPlayVideoWithDictionary(string dic)
    {
        everyplayUnity.Call("playVideoWithDictionary", dic);
    }

    public static string EveryplayAccountAccessToken()
    {
        return everyplayUnity.Call<string>("getAccessToken");
    }

    public static void EveryplayShowSharingModal()
    {
        everyplayUnity.Call("showSharingModal");
    }

    public static void EveryplayStartRecording()
    {
        everyplayUnity.Call("startRecording");
    }

    public static void EveryplayStopRecording()
    {
        everyplayUnity.Call("stopRecording");
    }

    public static void EveryplayPauseRecording()
    {
        everyplayUnity.Call("pauseRecording");
    }

    public static void EveryplayResumeRecording()
    {
        everyplayUnity.Call("resumeRecording");
    }

    public static bool EveryplayIsRecording()
    {
        return everyplayUnity.Call<bool>("isRecording");
    }

    public static bool EveryplayIsRecordingSupported()
    {
        return everyplayUnity.Call<bool>("isRecordingSupported");
    }

    public static bool EveryplayIsPaused()
    {
        return everyplayUnity.Call<bool>("isPaused");
    }

    public static bool EveryplaySnapshotRenderbuffer()
    {
        return everyplayUnity.Call<bool>("snapshotRenderbuffer");
    }

    public static void EveryplayPlayLastRecording()
    {
        everyplayUnity.Call("playLastRecording");
    }

    public static void EveryplaySetMetadata(string json)
    {
        everyplayUnity.Call("setMetadata", json);
    }

    public static void EveryplaySetTargetFPS(int fps)
    {
        everyplayUnity.Call("setTargetFPS", fps);
    }

    public static void EveryplaySetMotionFactor(int factor)
    {
        everyplayUnity.Call("setMotionFactor", factor);
    }

    public static void EveryplaySetMaxRecordingMinutesLength(int minutes)
    {
        everyplayUnity.Call("setMaxRecordingMinutesLength", minutes);
    }

    public static void EveryplaySetLowMemoryDevice(bool state)
    {
        everyplayUnity.Call("setLowMemoryDevice", state ? 1 : 0);
    }

    public static void EveryplaySetDisableSingleCoreDevices(bool state)
    {
        everyplayUnity.Call("setDisableSingleCoreDevices", state ? 1 : 0);
    }

    public static bool EveryplayIsSupported()
    {
        return everyplayUnity.Call<bool>("isSupported");
    }

    public static bool EveryplayIsSingleCoreDevice()
    {
        return everyplayUnity.Call<bool>("isSingleCoreDevice");
    }

    public static int EveryplayGetUserInterfaceIdiom()
    {
        return everyplayUnity.Call<int>("getUserInterfaceIdiom");
    }

    public static bool EveryplayFaceCamIsVideoRecordingSupported()
    {
        return false;
    }

    public static bool EveryplayFaceCamIsAudioRecordingSupported()
    {
        return false;
    }

    public static bool EveryplayFaceCamIsHeadphonesPluggedIn()
    {
        return false;
    }

    public static bool EveryplayFaceCamIsSessionRunning()
    {
        return false;
    }

    public static bool EveryplayFaceCamIsRecordingPermissionGranted()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
        return false;
    }

    public static float EveryplayFaceCamAudioPeakLevel()
    {
        return 0.0f;
    }

    public static float EveryplayFaceCamAudioPowerLevel()
    {
        return 0.0f;
    }

    public static void EveryplayFaceCamSetMonitorAudioLevels(bool enabled)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetAudioOnly(bool audioOnly)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewVisible(bool visible)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewScaleRetina(bool autoScale)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewSideWidth(int width)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewBorderWidth(int width)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewPositionX(int x)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewPositionY(int y)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewBorderColor(float r, float g, float b, float a)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetPreviewOrigin(int origin)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetTargetTextureId(int textureId)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetTargetTextureWidth(int textureHeight)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static void EveryplayFaceCamSetTargetTextureHeight(int textureWidth)
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static bool EveryplayFaceCamStartSession()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
        return false;
    }

    public static void EveryplayFaceCamRequestRecordingPermission()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
    }

    public static bool EveryplayFaceCamStopSession()
    {
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " not yet implemented");
        return false;
    }

    public static void EveryplaySetThumbnailWidth(int thumbnailWidth)
    {
        everyplayUnity.Call("setThumbnailWidth", thumbnailWidth);
    }

    public static void EveryplaySetThumbnailTargetTextureId(int textureId)
    {
        everyplayUnity.Call("setThumbnailTargetTextureId", textureId);
    }

    public static void EveryplaySetThumbnailTargetTextureWidth(int textureWidth)
    {
        everyplayUnity.Call("setThumbnailTargetTextureWidth", textureWidth);
    }

    public static void EveryplaySetThumbnailTargetTextureHeight(int textureHeight)
    {
        everyplayUnity.Call("setThumbnailTargetTextureHeight", textureHeight);
    }

    public static void EveryplayTakeThumbnail()
    {
        everyplayUnity.Call("takeThumbnail");
    }

    #endif
}
