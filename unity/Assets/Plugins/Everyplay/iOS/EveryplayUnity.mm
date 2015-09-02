/*
 * Copyright 2012 Applifier
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

#import "EveryplayUnity.h"

#define EVERYPLAY_GLES_WRAPPER
#import "EveryplayGlesSupport.h"

void UnitySendMessage(const char* obj, const char* method, const char* msg);

extern "C" {
    char* EveryplayCopyString (const char* string) {
        if (string != NULL) {
            char* res = strdup(string);
            return res;
        }

        return NULL;
    }

    NSString* EveryplayCreateNSString (const char* string) {
        return string ? [NSString stringWithUTF8String: string] : [NSString stringWithUTF8String: ""];
    }

    NSURL* EveryplayCreateNSURL (const char* string) {
        return [NSURL URLWithString:EveryplayCreateNSString(string)];
    }

}

static EveryplayUnity * everyplayUnity = [EveryplayUnity sharedInstance];

@implementation EveryplayUnity

+ (void)initialize {
    if(everyplayUnity == nil) {
        everyplayUnity = [[EveryplayUnity alloc] init];
    }
}

+ (EveryplayUnity *) sharedInstance {
    return everyplayUnity;
}

- (id)init {
    if(everyplayUnity != nil) {
        return everyplayUnity;
    }

    self = [super init];
    if(self) {
        everyplayUnity = self;
        displayLinkPaused = NO;

#if UNITY_VERSION >= 430
        UnityRegisterAppDelegateListener(self);
#endif
    }
    return self;
}

- (void)setClientId:(NSString *)clientId andClientSecret:(NSString *)clientSecret andRedirectURI:(NSString *)redirectURI {
    [Everyplay initWithDelegate: self andParentViewController: UnityGetGLViewController()];
    [Everyplay setClientId: clientId clientSecret: clientSecret redirectURI: redirectURI];

    EveryplayLog(@"Everyplay init from Unity with client ID: %@ and client secret: %@ and redirect URI: %@", clientId, clientSecret, redirectURI);
}

- (void)everyplayShown {
    ELOG;
    currentOrientation = UnityGetGLViewController().interfaceOrientation;
    UnityPause(true);
#if UNITY_VERSION < 450
    CADisplayLink *displayLink = (CADisplayLink *) _displayLink;
#else
    CADisplayLink *displayLink = (CADisplayLink *) GetAppController().unityDisplayLink;
#endif
    if(displayLink != nil) {
        if([displayLink isPaused] == NO) {
            displayLinkPaused = YES;
            [displayLink setPaused: YES];
            EveryplayLog(@"Everyplay paused _displayLink");
        }
    }
}

- (void)everyplayHidden {
    ELOG;
#if UNITY_VERSION < 450
    CADisplayLink *displayLink = (CADisplayLink *) _displayLink;
#else
    CADisplayLink *displayLink = (CADisplayLink *) GetAppController().unityDisplayLink;
#endif
    if(displayLink != nil && displayLinkPaused) {
        displayLinkPaused = NO;
        [displayLink setPaused: NO];
        EveryplayLog(@"Everyplay unpaused _displaylink");
    }
    UnityPause(false);

    /* Force orientation check, orientation could have changed while Unity was paused */
    UIInterfaceOrientation newOrientation = UnityGetGLViewController().interfaceOrientation;
    if (currentOrientation != newOrientation) {
#if UNITY_VERSION <= 450
        ScreenOrientation orientation = ConvertToUnityScreenOrientation(newOrientation, 0);
        UnitySetScreenOrientation(orientation);
#endif
#if UNITY_VERSION >= 400
        UnityGLInvalidateState();
#endif
    }

    UnitySendMessage("Everyplay", "EveryplayHidden", "");
}

- (void)everyplayReadyForRecording:(NSNumber *)enabled {
    ELOG;
    NSString *jsonMsg = [NSString stringWithFormat: @"{ \"enabled\":%d }", [enabled intValue]];
    UnitySendMessage("Everyplay", "EveryplayReadyForRecording", [jsonMsg UTF8String]);
}

- (void)everyplayRecordingStarted {
    ELOG;
    UnitySendMessage("Everyplay", "EveryplayRecordingStarted", "");
}

- (void)everyplayRecordingStopped {
    ELOG;
    UnitySendMessage("Everyplay", "EveryplayRecordingStopped", "");
}

- (void)everyplayFaceCamSessionStarted {
    ELOG;
    UnitySendMessage("Everyplay", "EveryplayFaceCamSessionStarted", "");
}

- (void)everyplayFaceCamRecordingPermission:(NSNumber *)granted {
    ELOG;
    NSString *jsonMsg = [NSString stringWithFormat: @"{ \"granted\":%d }", [granted intValue]];
    UnitySendMessage("Everyplay", "EveryplayFaceCamRecordingPermission", [jsonMsg UTF8String]);
}

- (void)everyplayFaceCamSessionStopped {
    ELOG;
    UnitySendMessage("Everyplay", "EveryplayFaceCamSessionStopped", "");
}

- (void)everyplayThumbnailReadyAtFilePath:(NSString *)thumbnailFilePath {
    ELOG;
    NSString *jsonMsg = [NSString stringWithFormat: @"{ \"thumbnailFilePath\":\"%@\" }", (thumbnailFilePath == nil) ? @"" : thumbnailFilePath];
    UnitySendMessage("Everyplay", "EveryplayThumbnailReadyAtFilePath", [jsonMsg UTF8String]);
}

- (void)everyplayThumbnailReadyAtTextureId:(NSNumber *)textureId portraitMode: (NSNumber *) portrait {
    ELOG;
    NSString *jsonMsg = [NSString stringWithFormat: @"{ \"textureId\":%d,\"portrait\":%d }", [textureId intValue], [portrait intValue]];
    UnitySendMessage("Everyplay", "EveryplayThumbnailReadyAtTextureId", [jsonMsg UTF8String]);
}

- (void)everyplayUploadDidStart:(NSNumber *)videoId {
    ELOG;
    NSString *jsonMsg = [NSString stringWithFormat: @"{ \"videoId\":%d }", [videoId intValue]];
    UnitySendMessage("Everyplay", "EveryplayUploadDidStart", [jsonMsg UTF8String]);
}

- (void)everyplayUploadDidProgress:(NSNumber *)videoId progress:(NSNumber *)progress {
    ELOG;
    NSString *jsonMsg = [NSString stringWithFormat: @"{ \"videoId\":%d,\"progress\":%f }", [videoId intValue], [progress floatValue]];
    UnitySendMessage("Everyplay", "EveryplayUploadDidProgress", [jsonMsg UTF8String]);
}

- (void)everyplayUploadDidComplete:(NSNumber *)videoId {
    ELOG;
    NSString *jsonMsg = [NSString stringWithFormat: @"{ \"videoId\":%d }", [videoId intValue]];
    UnitySendMessage("Everyplay", "EveryplayUploadDidComplete", [jsonMsg UTF8String]);
}

#if UNITY_VERSION >= 430
- (void)onOpenURL:(NSNotification*)notification {
    NSLog(@"onOpenURL notification received");
    [Everyplay handleOpenURL:[notification.userInfo objectForKey:@"url"] sourceApplication:[notification.userInfo objectForKey:@"sourceApplication"] annotation:nil];
}
#endif

@end

extern "C" {
    void InitEveryplay(const char *clientId, const char *clientSecret, const char *redirectURI) {
        if(everyplayUnity != nil) {
            [everyplayUnity setClientId: EveryplayCreateNSString(clientId) andClientSecret: EveryplayCreateNSString(clientSecret) andRedirectURI: EveryplayCreateNSString(redirectURI)];
        }
    }

    void EveryplayShow() {
        [[Everyplay sharedInstance] showEveryplay];
    }

    void EveryplayShowWithPath(const char *path) {
        NSString *pathString = EveryplayCreateNSString(path);
        [[Everyplay sharedInstance] showEveryplayWithPath:pathString];
    }

    void EveryplayShowSharingModal() {
        [[Everyplay sharedInstance] showEveryplaySharingModal];
    }

    void EveryplayPlayVideoWithURL(const char *url) {
        NSURL *urlUrl = EveryplayCreateNSURL(url);
        [[Everyplay sharedInstance] playVideoWithURL:urlUrl];
    }

    void EveryplayPlayVideoWithDictionary(const char *dic) {
        Class jsonSerializationClass = NSClassFromString(@"NSJSONSerialization");

        if (!jsonSerializationClass) {
            return;
        }

        NSString *strValue = EveryplayCreateNSString(dic);

        NSError *jsonError = nil;
        NSData *jsonData = [strValue dataUsingEncoding:NSUTF8StringEncoding];

        id jsonParsedObj = [jsonSerializationClass JSONObjectWithData:jsonData options:0 error:&jsonError];

        if (jsonError == nil) {
            if ([jsonParsedObj isKindOfClass:[NSDictionary class]]) {
                [[Everyplay sharedInstance] playVideoWithDictionary: (NSDictionary *) jsonParsedObj];
            }
        } else {
            EveryplayLog(@"Failed parsing JSON: %@", jsonError);
        }

    }

    char* EveryplayAccountAccessToken() {
        return EveryplayCopyString([[[Everyplay account] accessToken] UTF8String]);
    }

    void EveryplayStartRecording() {
        [[[Everyplay sharedInstance] capture] startRecording];
    }

    void EveryplayStopRecording() {
        [[[Everyplay sharedInstance] capture] stopRecording];
    }

    void EveryplayPauseRecording() {
        [[[Everyplay sharedInstance] capture] pauseRecording];
    }

    void EveryplayResumeRecording() {
        [[[Everyplay sharedInstance] capture] resumeRecording];
    }

    bool EveryplayIsRecording() {
        return [[[Everyplay sharedInstance] capture] isRecording];
    }

    bool EveryplayIsRecordingSupported() {
        return [[[Everyplay sharedInstance] capture] isRecordingSupported];
    }

    bool EveryplayIsPaused() {
        return [[[Everyplay sharedInstance] capture] isPaused];
    }

    bool EveryplaySnapshotRenderbuffer() {
        return [[[Everyplay sharedInstance] capture] snapshotRenderbuffer];
    }

    void EveryplayPlayLastRecording() {
        [[Everyplay sharedInstance] playLastRecording];
    }

    void EveryplaySetMetadata(const char *val) {
        Class jsonSerializationClass = NSClassFromString(@"NSJSONSerialization");

        if (!jsonSerializationClass) {
            return;
        }

        NSString *strValue = EveryplayCreateNSString(val);

        EveryplayLog(@"Set metadata %@", strValue);

        NSError *jsonError = nil;
        NSData *jsonData = [strValue dataUsingEncoding:NSUTF8StringEncoding];

        id jsonParsedObj = [jsonSerializationClass JSONObjectWithData:jsonData options:0 error:&jsonError];

        if (jsonError == nil) {
            if ([jsonParsedObj isKindOfClass:[NSDictionary class]]) {
                [[Everyplay sharedInstance] mergeSessionDeveloperData: (NSDictionary *) jsonParsedObj];
            }
        } else {
            EveryplayLog(@"Failed parsing JSON: %@", jsonError);
        }
    }

    void EveryplaySetTargetFPS(int fps) {
        [[Everyplay sharedInstance] capture].targetFPS = fps;
    }

    void EveryplaySetMotionFactor(int factor) {
        [[Everyplay sharedInstance] capture].motionFactor = factor;
    }

    void EveryplaySetMaxRecordingMinutesLength(int minutes) {
        [[Everyplay sharedInstance] capture].maxRecordingMinutesLength = minutes;
    }

    void EveryplaySetLowMemoryDevice(bool state) {
        [[Everyplay sharedInstance] capture].lowMemoryDevice = state;
    }

    void EveryplaySetDisableSingleCoreDevices(bool state) {
        [[Everyplay sharedInstance] capture].disableSingleCoreDevices = state;
    }

    bool EveryplayIsSupported() {
        return [Everyplay isSupported];
    }

    bool EveryplayIsSingleCoreDevice() {
        return [[[Everyplay sharedInstance] capture] isSingleCoreDevice];
    }

    bool EveryplayFaceCamIsVideoRecordingSupported() {
        return [[[Everyplay sharedInstance] faceCam] isVideoRecordingSupported];
    }

    bool EveryplayFaceCamIsAudioRecordingSupported() {
        return [[[Everyplay sharedInstance] faceCam] isAudioRecordingSupported];
    }

    bool EveryplayFaceCamIsHeadphonesPluggedIn() {
        return [[[Everyplay sharedInstance] faceCam] isHeadphonesPluggedIn];
    }

    bool EveryplayFaceCamIsSessionRunning() {
        return [[[Everyplay sharedInstance] faceCam] isSessionRunning];
    }

    bool EveryplayFaceCamIsRecordingPermissionGranted() {
        return [[[Everyplay sharedInstance] faceCam] isRecordingPermissionGranted];
    }

    int EveryplayGetUserInterfaceIdiom() {
        return [[UIDevice currentDevice] userInterfaceIdiom];
    }

    float EveryplayFaceCamAudioPeakLevel() {
        return [[[Everyplay sharedInstance] faceCam] audioPeakLevel];
    }

    float EveryplayFaceCamAudioPowerLevel() {
        return [[[Everyplay sharedInstance] faceCam] audioPowerLevel];
    }

    void EveryplayFaceCamSetMonitorAudioLevels(bool enabled) {
        [[[Everyplay sharedInstance] faceCam] setMonitorAudioLevels: enabled];
    }

    void EveryplayFaceCamSetAudioOnly(bool audioOnly) {
        [[[Everyplay sharedInstance] faceCam] setAudioOnly: audioOnly];
    }

    void EveryplayFaceCamSetPreviewVisible(bool visible) {
        [[[Everyplay sharedInstance] faceCam] setPreviewVisible: visible];
    }

    void EveryplayFaceCamSetPreviewScaleRetina(bool autoScale) {
        [[[Everyplay sharedInstance] faceCam] setPreviewScaleRetina: autoScale];
    }

    void EveryplayFaceCamSetPreviewSideWidth(int width) {
        [[[Everyplay sharedInstance] faceCam] setPreviewSideWidth: width];
    }

    void EveryplayFaceCamSetPreviewBorderWidth(int width) {
        [[[Everyplay sharedInstance] faceCam] setPreviewBorderWidth: width];
    }

    void EveryplayFaceCamSetPreviewPositionX(int x) {
        [[[Everyplay sharedInstance] faceCam] setPreviewPositionX: x];
    }

    void EveryplayFaceCamSetPreviewPositionY(int y) {
        [[[Everyplay sharedInstance] faceCam] setPreviewPositionY: y];
    }

    void EveryplayFaceCamSetPreviewOrigin(int origin) {
        [[[Everyplay sharedInstance] faceCam] setPreviewOrigin: static_cast<EveryplayFaceCamPreviewOrigin>(origin)];
    }

    void EveryplayFaceCamSetPreviewBorderColor(float r, float g, float b, float a) {
        EveryplayFaceCamColor color = { .r = r, .g = g, .b = b, .a = a };
        [[[Everyplay sharedInstance] faceCam] setPreviewBorderColor: color];
    }

    void EveryplayFaceCamSetTargetTextureId(int textureId) {
        [[[Everyplay sharedInstance] faceCam] setTargetTextureId: textureId];
    }

    void EveryplayFaceCamSetTargetTextureWidth(int textureWidth) {
        [[[Everyplay sharedInstance] faceCam] setTargetTextureWidth: textureWidth];
    }

    void EveryplayFaceCamSetTargetTextureHeight(int textureHeight) {
        [[[Everyplay sharedInstance] faceCam] setTargetTextureHeight: textureHeight];
    }

    void EveryplayFaceCamStartSession() {
        [[[Everyplay sharedInstance] faceCam] startSession];
    }

    void EveryplayFaceCamRequestRecordingPermission() {
        [[[Everyplay sharedInstance] faceCam] requestRecordingPermission];
    }

    void EveryplayFaceCamStopSession() {
        [[[Everyplay sharedInstance] faceCam] stopSession];
    }

    void EveryplaySetThumbnailWidth(int thumbnailWidth) {
        [[[Everyplay sharedInstance] capture] setThumbnailWidth: thumbnailWidth];
    }

    void EveryplaySetThumbnailTargetTextureId(int textureId) {
        [[[Everyplay sharedInstance] capture] setThumbnailTargetTextureId: textureId];
    }

    void EveryplaySetThumbnailTargetTextureWidth(int textureWidth) {
        [[[Everyplay sharedInstance] capture] setThumbnailTargetTextureWidth: textureWidth];
    }

    void EveryplaySetThumbnailTargetTextureHeight(int textureHeight) {
        [[[Everyplay sharedInstance] capture] setThumbnailTargetTextureHeight: textureHeight];
    }

    void EveryplayTakeThumbnail() {
        [[[Everyplay sharedInstance] capture] takeThumbnail];
    }
}
