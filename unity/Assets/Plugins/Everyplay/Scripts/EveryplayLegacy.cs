using UnityEngine;
using System.Collections.Generic;

public class EveryplayLegacy : MonoBehaviour
{
    public event Everyplay.WasClosedDelegate WasClosed {
        add {
            Everyplay.WasClosed += value;
        }
        remove {
            Everyplay.WasClosed -= value;
        }
    }

    public event Everyplay.ReadyForRecordingDelegate ReadyForRecording {
        add {
            Everyplay.ReadyForRecording += value;
        }
        remove {
            Everyplay.ReadyForRecording -= value;
        }
    }

    public event Everyplay.RecordingStartedDelegate RecordingStarted {
        add {
            Everyplay.RecordingStarted += value;
        }
        remove {
            Everyplay.RecordingStarted -= value;
        }
    }

    public event Everyplay.RecordingStoppedDelegate RecordingStopped {
        add {
            Everyplay.RecordingStopped += value;
        }
        remove {
            Everyplay.RecordingStopped -= value;
        }
    }

    public event Everyplay.FaceCamSessionStartedDelegate FaceCamSessionStarted {
        add {
            Everyplay.FaceCamSessionStarted += value;
        }
        remove {
            Everyplay.FaceCamSessionStarted -= value;
        }
    }

    public event Everyplay.FaceCamRecordingPermissionDelegate FaceCamRecordingPermission {
        add {
            Everyplay.FaceCamRecordingPermission += value;
        }
        remove {
            Everyplay.FaceCamRecordingPermission -= value;
        }
    }

    public event Everyplay.FaceCamSessionStoppedDelegate FaceCamSessionStopped {
        add {
            Everyplay.FaceCamSessionStopped += value;
        }
        remove {
            Everyplay.FaceCamSessionStopped -= value;
        }
    }

    public event Everyplay.ThumbnailReadyAtFilePathDelegate ThumbnailReadyAtFilePath {
        add {
            Everyplay.ThumbnailReadyAtFilePath += value;
        }
        remove {
            Everyplay.ThumbnailReadyAtFilePath -= value;
        }
    }

    public event Everyplay.ThumbnailReadyAtTextureIdDelegate ThumbnailReadyAtTextureId {
        add {
            Everyplay.ThumbnailReadyAtTextureId += value;
        }
        remove {
            Everyplay.ThumbnailReadyAtTextureId -= value;
        }
    }

    public event Everyplay.UploadDidStartDelegate UploadDidStart {
        add {
            Everyplay.UploadDidStart += value;
        }
        remove {
            Everyplay.UploadDidStart -= value;
        }
    }

    public event Everyplay.UploadDidProgressDelegate UploadDidProgress {
        add {
            Everyplay.UploadDidProgress += value;
        }
        remove {
            Everyplay.UploadDidProgress -= value;
        }
    }

    public event Everyplay.UploadDidCompleteDelegate UploadDidComplete {
        add {
            Everyplay.UploadDidComplete += value;
        }
        remove {
            Everyplay.UploadDidComplete -= value;
        }
    }

    public void Show()
    {
        Everyplay.Show();
    }

    public void ShowWithPath(string path)
    {
        Everyplay.ShowWithPath(path);
    }

    public void PlayVideoWithURL(string url)
    {
        Everyplay.PlayVideoWithURL(url);
    }

    public void PlayVideoWithDictionary(Dictionary<string,object> dict)
    {
        Everyplay.PlayVideoWithDictionary(dict);
    }

    public void MakeRequest(string method, string url, Dictionary<string, object> data, Everyplay.RequestReadyDelegate readyDelegate, Everyplay.RequestFailedDelegate failedDelegate)
    {
        Everyplay.MakeRequest(method, url, data, readyDelegate, failedDelegate);
    }

    public string AccessToken()
    {
        return Everyplay.AccessToken();
    }

    public void ShowSharingModal()
    {
        Everyplay.ShowSharingModal();
    }

    public void StartRecording()
    {
        Everyplay.StartRecording();
    }

    public void StopRecording()
    {
        Everyplay.StopRecording();
    }

    public void PauseRecording()
    {
        Everyplay.PauseRecording();
    }

    public void ResumeRecording()
    {
        Everyplay.ResumeRecording();
    }

    public bool IsRecording()
    {
        return Everyplay.IsRecording();
    }

    public bool IsRecordingSupported()
    {
        return Everyplay.IsRecordingSupported();
    }

    public bool IsPaused()
    {
        return Everyplay.IsPaused();
    }

    public bool SnapshotRenderbuffer()
    {
        return Everyplay.SnapshotRenderbuffer();
    }

    public bool IsSupported()
    {
        return Everyplay.IsSupported();
    }

    public bool IsSingleCoreDevice()
    {
        return Everyplay.IsSingleCoreDevice();
    }

    public int GetUserInterfaceIdiom()
    {
        return Everyplay.GetUserInterfaceIdiom();
    }

    public void PlayLastRecording()
    {
        Everyplay.PlayLastRecording();
    }

    public void SetMetadata(string key, object val)
    {
        Everyplay.SetMetadata(key, val);
    }

    public void SetMetadata(Dictionary<string,object> dict)
    {
        Everyplay.SetMetadata(dict);
    }

    public void SetTargetFPS(int fps)
    {
        Everyplay.SetTargetFPS(fps);
    }

    public void SetMotionFactor(int factor)
    {
        Everyplay.SetMotionFactor(factor);
    }

    public void SetMaxRecordingMinutesLength(int minutes)
    {
        Everyplay.SetMaxRecordingMinutesLength(minutes);
    }

    public void SetLowMemoryDevice(bool state)
    {
        Everyplay.SetLowMemoryDevice(state);
    }

    public void SetDisableSingleCoreDevices(bool state)
    {
        Everyplay.SetDisableSingleCoreDevices(state);
    }

    public void LoadThumbnailFromFilePath(string filePath, Everyplay.ThumbnailLoadReadyDelegate readyDelegate, Everyplay.ThumbnailLoadFailedDelegate failedDelegate)
    {
        Everyplay.LoadThumbnailFromFilePath(filePath, readyDelegate, failedDelegate);
    }

    public bool FaceCamIsVideoRecordingSupported()
    {
        return Everyplay.FaceCamIsVideoRecordingSupported();
    }

    public bool FaceCamIsAudioRecordingSupported()
    {
        return Everyplay.FaceCamIsAudioRecordingSupported();
    }

    public bool FaceCamIsHeadphonesPluggedIn()
    {
        return Everyplay.FaceCamIsHeadphonesPluggedIn();
    }

    public bool FaceCamIsSessionRunning()
    {
        return Everyplay.FaceCamIsSessionRunning();
    }

    public bool FaceCamIsRecordingPermissionGranted()
    {
        return Everyplay.FaceCamIsRecordingPermissionGranted();
    }

    public float FaceCamAudioPeakLevel()
    {
        return Everyplay.FaceCamAudioPeakLevel();
    }

    public float FaceCamAudioPowerLevel()
    {
        return Everyplay.FaceCamAudioPowerLevel();
    }

    public void FaceCamSetMonitorAudioLevels(bool enabled)
    {
        Everyplay.FaceCamSetMonitorAudioLevels(enabled);
    }

    public void FaceCamSetAudioOnly(bool audioOnly)
    {
        Everyplay.FaceCamSetAudioOnly(audioOnly);
    }

    public void FaceCamSetPreviewVisible(bool visible)
    {
        Everyplay.FaceCamSetPreviewVisible(visible);
    }

    public void FaceCamSetPreviewScaleRetina(bool autoScale)
    {
        Everyplay.FaceCamSetPreviewScaleRetina(autoScale);
    }

    public void FaceCamSetPreviewSideWidth(int width)
    {
        Everyplay.FaceCamSetPreviewSideWidth(width);
    }

    public void FaceCamSetPreviewBorderWidth(int width)
    {
        Everyplay.FaceCamSetPreviewBorderWidth(width);
    }

    public void FaceCamSetPreviewPositionX(int x)
    {
        Everyplay.FaceCamSetPreviewPositionX(x);
    }

    public void FaceCamSetPreviewPositionY(int y)
    {
        Everyplay.FaceCamSetPreviewPositionY(y);
    }

    public void FaceCamSetPreviewBorderColor(float r, float g, float b, float a)
    {
        Everyplay.FaceCamSetPreviewBorderColor(r, g, b, a);
    }

    public void FaceCamSetPreviewOrigin(Everyplay.FaceCamPreviewOrigin origin)
    {
        Everyplay.FaceCamSetPreviewOrigin(origin);
    }

    public void SetThumbnailWidth(int thumbnailWidth)
    {
        Everyplay.SetThumbnailWidth(thumbnailWidth);
    }

    public void FaceCamSetTargetTextureId(int textureId)
    {
        Everyplay.FaceCamSetTargetTextureId(textureId);
    }

    public void FaceCamSetTargetTextureWidth(int textureWidth)
    {
        Everyplay.FaceCamSetTargetTextureWidth(textureWidth);
    }

    public void FaceCamSetTargetTextureHeight(int textureHeight)
    {
        Everyplay.FaceCamSetTargetTextureHeight(textureHeight);
    }

    public void FaceCamStartSession()
    {
        Everyplay.FaceCamStartSession();
    }

    public void FaceCamRequestRecordingPermission()
    {
        Everyplay.FaceCamRequestRecordingPermission();
    }

    public void FaceCamStopSession()
    {
        Everyplay.FaceCamStopSession();
    }

    public void SetThumbnailTargetTextureId(int textureId)
    {
        Everyplay.SetThumbnailTargetTextureId(textureId);
    }

    public void SetThumbnailTargetTextureWidth(int textureWidth)
    {
        Everyplay.SetThumbnailTargetTextureWidth(textureWidth);
    }

    public void SetThumbnailTargetTextureHeight(int textureHeight)
    {
        Everyplay.SetThumbnailTargetTextureHeight(textureHeight);
    }

    public void TakeThumbnail()
    {
        Everyplay.TakeThumbnail();
    }
}
