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

#ifndef UNITY_VERSION
#include "EveryplayConfig.h"
#define UNITY_VERSION EVERYPLAY_UNITY_VERSION
#endif

#if !defined(UNITY_VERSION) || UNITY_VERSION == 0
#error "Everyplay integration error"
#error "Try rebuilding and replacing the Xcode project from scratch in Unity"
#error "Make sure all Assets/Editor/PostprocessBuildPlayer* files are run correctly"
#endif

#if UNITY_VERSION >= 410
#include "GlesHelper.h"
#include "EAGLContextHelper.h"
#else
#include "iPhone_GlesSupport.h"
#endif

#if UNITY_VERSION >= 450
#define EVERYPLAY_UNITY_GLES_INTEGRATION 0
#else
#define EVERYPLAY_UNITY_GLES_INTEGRATION 1
#endif

// Original GLES methods

#if EVERYPLAY_UNITY_GLES_INTEGRATION
#if UNITY_VERSION >= 410
void CreateUnityRenderBuffers_Unity(UnityRenderingSurface* surface);
void DestroySystemRenderingSurface_Unity(UnityRenderingSurface* surface);
void PreparePresentRenderingSurface_Unity(UnityRenderingSurface* surface, EAGLContext* mainContext);
void SetupUnityDefaultFBO_Unity(UnityRenderingSurface* surface);
extern "C" bool UnityResolveMSAA_Unity(GLuint destFBO, GLuint colorTex, GLuint colorBuf, GLuint depthTex, GLuint depthBuf);
#else
void CreateSurfaceGLES_Unity(EAGLSurfaceDesc* surface);
void DestroySurfaceGLES_Unity(EAGLSurfaceDesc* surface);
void PreparePresentSurfaceGLES_Unity(EAGLSurfaceDesc* surface);
void AfterPresentSurfaceGLES_Unity(EAGLSurfaceDesc* surface);
extern "C" bool UnityResolveMSAA_Unity(GLuint destFBO, GLuint colorTex, GLuint colorBuf, GLuint depthTex, GLuint depthBuf);
#endif
#endif

#ifndef UNITY_DBG_LOG

#define ENABLE_UNITY_DEBUG_LOG  0

#if ENABLE_UNITY_DEBUG_LOG
#define UNITY_DBG_LOG(...) \
  do { \
    printf_console(__VA_ARGS__); \
  } while(0)
#else
#define UNITY_DBG_LOG(...) \
  do { \
  } while(0)
#endif
#endif

#ifdef EVERYPLAY_GLES_WRAPPER

#include "iPhone_Profiler.h"
#if UNITY_VERSION >= 420
#import "UnityAppController.h"
#else
#import "AppController.h"
#endif

// iPhone_View.h
extern UIViewController* UnityGetGLViewController();
extern UIView* UnityGetGLView();

// AppController.m
#if UNITY_VERSION < 410
extern EAGLSurfaceDesc _surface;
#else
extern "C" const UnityRenderingSurface* UnityDisplayManager_MainDisplayRenderingSurface();
#endif
extern id _displayLink;

extern "C" void InitEAGLLayer(void* eaglLayer, bool use32bitColor);
#if UNITY_VERSION < 410
extern "C" bool AllocateRenderBufferStorageFromEAGLLayer(void* eaglLayer);
extern "C" void DeallocateRenderBufferStorageFromEAGLLayer();
#endif

// iPhone_GlesSupport.cpp
extern bool	UnityIsCaptureScreenshotRequested();
extern void	UnityCaptureScreenshot();

// libiPhone
extern GLint gDefaultFBO;

extern bool UnityUse32bitDisplayBuffer();
extern int UnityGetDesiredMSAASampleCount(int defaultSampleCount);
extern void UnityGetRenderingResolution(unsigned* w, unsigned* h);

extern void UnityBlitToSystemFB(unsigned tex, unsigned w, unsigned h, unsigned sysw, unsigned sysh);

extern void UnityPause(bool pause);

#if UNITY_VERSION == 350

enum EnabledOrientation
{
    kAutorotateToPortrait = 1,
    kAutorotateToPortraitUpsideDown = 2,
    kAutorotateToLandscapeLeft = 4,
    kAutorotateToLandscapeRight = 8
};

enum ScreenOrientation
{
    kScreenOrientationUnknown,
    portrait,
    portraitUpsideDown,
    landscapeLeft,
    landscapeRight,
    autorotation,
    kScreenOrientationCount
};
#else
#import "iPhone_OrientationSupport.h"
extern void UnityGLInvalidateState();
#endif

#if UNITY_VERSION >= 430
extern ScreenOrientation ConvertToUnityScreenOrientation(UIInterfaceOrientation hwOrient, EnabledOrientation* outAutorotOrient);
extern void UnitySetScreenOrientation(int/*ScreenOrientation*/ orientation);
#else
extern ScreenOrientation ConvertToUnityScreenOrientation(int hwOrient, EnabledOrientation* outAutorotOrient);
extern void UnitySetScreenOrientation(ScreenOrientation orientation);
#endif

#else

#if EVERYPLAY_UNITY_GLES_INTEGRATION
#if UNITY_VERSION >= 410
#define CreateUnityRenderBuffers CreateUnityRenderBuffers_Unity
#define DestroySystemRenderingSurface DestroySystemRenderingSurface_Unity
#define PreparePresentRenderingSurface PreparePresentRenderingSurface_Unity
#define SetupUnityDefaultFBO SetupUnityDefaultFBO_Unity
#define UnityResolveMSAA UnityResolveMSAA_Unity
#else
#define CreateSurfaceGLES CreateSurfaceGLES_Unity
#define DestroySurfaceGLES DestroySurfaceGLES_Unity
#define PreparePresentSurfaceGLES PreparePresentSurfaceGLES_Unity
#define AfterPresentSurfaceGLES AfterPresentSurfaceGLES_Unity
#define UnityResolveMSAA UnityResolveMSAA_Unity
#endif
#endif

#endif
