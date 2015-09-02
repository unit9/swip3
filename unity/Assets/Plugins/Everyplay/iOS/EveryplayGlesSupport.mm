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

#define EVERYPLAY_GLES_WRAPPER
#import "EveryplayGlesSupport.h"
#import <Everyplay/Everyplay.h>

#if !EVERYPLAY_UNITY_GLES_INTEGRATION

#if !defined(EVERYPLAY_CAPTURE_API_VERSION) || EVERYPLAY_CAPTURE_API_VERSION <= 1
#error "Everyplay SDK 1.7.6 or later required"
#endif

#else

#if defined(EVERYPLAY_CAPTURE_API_VERSION) && EVERYPLAY_CAPTURE_API_VERSION >= 2
#define ENABLE_GLES_WRAPPER 0
#else
#define ENABLE_GLES_WRAPPER 1
#endif

#if ENABLE_GLES_WRAPPER
EveryplayCapture* everyplayCapture;
#endif

#if UNITY_VERSION < 410

void CreateSurfaceGLES(EAGLSurfaceDesc* surface)
{
#if ENABLE_GLES_WRAPPER
	DestroySurfaceGLES(surface);

	CreateSurfaceGLES_Unity(surface);

	if (!everyplayCapture) {
		everyplayCapture = [[EveryplayCapture alloc] initWithView:UnityGetGLView()
													  eaglContext:[EAGLContext currentContext]
														    layer:(CAEAGLLayer *)_surface.eaglLayer];
		[everyplayCapture setActiveFramebufferCallback:^(GLuint activeFramebuffer) {
			gDefaultFBO = activeFramebuffer;
		}];
	}

#if UNITY_VERSION == 350
	[everyplayCapture createFramebuffer:surface->framebuffer withMSAA:surface->msaaFramebuffer];
#elif UNITY_VERSION == 400
	GLuint drawFB = surface->targetFramebuffer ? surface->targetFramebuffer : surface->systemFramebuffer;
	[everyplayCapture createFramebuffer:drawFB withMSAA:surface->msaaFramebuffer];
#endif

#else
	CreateSurfaceGLES_Unity(surface);
#endif
}

void DestroySurfaceGLES(EAGLSurfaceDesc* surface)
{
#if ENABLE_GLES_WRAPPER
	[everyplayCapture deleteFramebuffer];
#endif
	DestroySurfaceGLES_Unity(surface);
}

void PreparePresentSurfaceGLES(EAGLSurfaceDesc* surface)
{
#if ENABLE_GLES_WRAPPER
#if UNITY_VERSION == 350
	BOOL isRecording = [everyplayCapture isRecording];

#if GL_APPLE_framebuffer_multisample
	if( !isRecording && surface->msaaSamples > 0 && _supportsMSAA )
	{
		Profiler_StartMSAAResolve();

		UNITY_DBG_LOG ("  ResolveMSAA: samples=%i msaaFBO=%i destFBO=%i\n", surface->msaaSamples, surface->msaaFramebuffer, surface->framebuffer);
		GLES_CHK( glBindFramebufferOES(GL_READ_FRAMEBUFFER_APPLE, surface->msaaFramebuffer) );
		GLES_CHK( glBindFramebufferOES(GL_DRAW_FRAMEBUFFER_APPLE, surface->framebuffer) );

		GLES_CHK( glResolveMultisampleFramebufferAPPLE() );

		Profiler_EndMSAAResolve();
	}
#endif

	// update screenshot here
	if( UnityIsCaptureScreenshotRequested() )
	{
		GLint curfb = 0;
		GLES_CHK( glGetIntegerv(GL_FRAMEBUFFER_BINDING, &curfb) );
		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, surface->framebuffer) );
		UnityCaptureScreenshot();
		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, curfb) );
	}

	[everyplayCapture beforePresentRenderbuffer:surface->framebuffer];

#if GL_EXT_discard_framebuffer
	if( !isRecording && _supportsDiscard )
	{
		GLenum attachments[3];
		int discardCount = 0;
		if (surface->msaaSamples > 1 && _supportsMSAA)
			attachments[discardCount++] = GL_COLOR_ATTACHMENT0_OES;

		if (surface->depthFormat)
			attachments[discardCount++] = GL_DEPTH_ATTACHMENT_OES;

		attachments[discardCount++] = GL_STENCIL_ATTACHMENT_OES;

		GLenum target = (surface->msaaSamples > 1 && _supportsMSAA) ? GL_READ_FRAMEBUFFER_APPLE: GL_FRAMEBUFFER_OES;
		if (discardCount > 0)
		{
			GLES_CHK( glDiscardFramebufferEXT(target, discardCount, attachments) );
		}
	}
#endif
#elif UNITY_VERSION == 400
	BOOL isRecording = [everyplayCapture isRecording];

#if GL_APPLE_framebuffer_multisample
	if( !isRecording && surface->msaaSamples > 1 && _supportsMSAA )
	{
		Profiler_StartMSAAResolve();

		GLuint drawFB = surface->targetFramebuffer ? surface->targetFramebuffer : surface->systemFramebuffer;

		GLES_CHK( glBindFramebufferOES(GL_READ_FRAMEBUFFER_APPLE, surface->msaaFramebuffer) );
		GLES_CHK( glBindFramebufferOES(GL_DRAW_FRAMEBUFFER_APPLE, drawFB) );
		GLES_CHK( glResolveMultisampleFramebufferAPPLE() );

		Profiler_EndMSAAResolve();
	}
#endif

	// update screenshot from target FBO to get requested resolution
	if( UnityIsCaptureScreenshotRequested() )
	{
		GLint target = surface->targetFramebuffer ? surface->targetFramebuffer : surface->systemFramebuffer;

		GLint curfb = 0;
		GLES_CHK( glGetIntegerv(GL_FRAMEBUFFER_BINDING, &curfb) );

		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, target) );
		UnityCaptureScreenshot();
		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, curfb) );
	}

	if( surface->targetFramebuffer )
	{
		[everyplayCapture beforePresentRenderbuffer:surface->targetFramebuffer];

		gDefaultFBO = surface->systemFramebuffer;
		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, gDefaultFBO) );

		UnityBlitToSystemFB(surface->targetRT, surface->targetW, surface->targetH, surface->systemW, surface->systemH);

		gDefaultFBO = surface->msaaFramebuffer ? surface->msaaFramebuffer : surface->targetFramebuffer;
		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, gDefaultFBO) );
	} else {
		[everyplayCapture beforePresentRenderbuffer:surface->systemFramebuffer];
	}

#if GL_EXT_discard_framebuffer
	if( !isRecording && _supportsDiscard )
	{
		GLenum	discardAttach[] = {GL_COLOR_ATTACHMENT0_OES, GL_DEPTH_ATTACHMENT_OES, GL_STENCIL_ATTACHMENT_OES};
		GLuint msaaFramebufferActive = [everyplayCapture msaaFramebuffer:surface->msaaFramebuffer];

		if( surface->msaaFramebuffer && surface->msaaFramebuffer == msaaFramebufferActive)
			GLES_CHK( glDiscardFramebufferEXT(GL_READ_FRAMEBUFFER_APPLE, 3, discardAttach) );

		if(surface->targetFramebuffer)
		{
			GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, surface->targetFramebuffer) );
			GLES_CHK( glDiscardFramebufferEXT(GL_FRAMEBUFFER_OES, 3, discardAttach) );
		}

		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, surface->systemFramebuffer) );
		GLES_CHK( glDiscardFramebufferEXT(GL_FRAMEBUFFER_OES, 2, &discardAttach[1]) );

		GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, gDefaultFBO) );
	}
#endif
#endif

#else
	PreparePresentSurfaceGLES_Unity(surface);
#endif
}

void AfterPresentSurfaceGLES(EAGLSurfaceDesc* surface)
{
#if ENABLE_GLES_WRAPPER
#if UNITY_VERSION == 350
	if(surface->use32bitColor != UnityUse32bitDisplayBuffer())
	{
		surface->use32bitColor = UnityUse32bitDisplayBuffer();
		CreateSurfaceGLES(surface);
		GLES_CHK( glBindRenderbufferOES(GL_RENDERBUFFER_OES, surface->renderbuffer) );
	}

#if GL_APPLE_framebuffer_multisample
	if (_supportsMSAA)
	{
		const int desiredMSAASamples = UnityGetDesiredMSAASampleCount(MSAA_DEFAULT_SAMPLE_COUNT);
		if (surface->msaaSamples != desiredMSAASamples)
		{
			surface->msaaSamples = desiredMSAASamples;
			CreateSurfaceGLES(surface);
			GLES_CHK( glBindRenderbufferOES(GL_RENDERBUFFER_OES, surface->renderbuffer) );
		}

		if (surface->msaaSamples > 1)
		{
			UNITY_DBG_LOG ("  glBindFramebufferOES (GL_FRAMEBUFFER_OES, %i); // PresentSurface\n", surface->msaaFramebuffer);
			GLES_CHK( glBindFramebufferOES(GL_FRAMEBUFFER_OES, surface->msaaFramebuffer) );

			gDefaultFBO = surface->msaaFramebuffer;
		}
	}
#endif

#elif UNITY_VERSION == 400
	if(surface->use32bitColor != UnityUse32bitDisplayBuffer())
	{
		surface->use32bitColor = UnityUse32bitDisplayBuffer();
		CreateSurfaceGLES(surface);
		GLES_CHK( glBindRenderbufferOES(GL_RENDERBUFFER_OES, surface->systemRenderbuffer) );
	}

	if(NeedRecreateRenderingSurfaceGLES(surface))
	{
		UnityGetRenderingResolution(&surface->targetW, &surface->targetH);
		surface->msaaSamples = UnityGetDesiredMSAASampleCount(MSAA_DEFAULT_SAMPLE_COUNT);

		CreateSurfaceGLES(surface);
	}
#endif

	if (_supportsMSAA && surface->msaaSamples > 1) {
		[everyplayCapture afterPresentRenderbuffer:surface->msaaFramebuffer];
	} else {
		[everyplayCapture afterPresentRenderbuffer];
	}
#else
	AfterPresentSurfaceGLES_Unity(surface);
#endif
}

extern "C" bool UnityResolveMSAA(GLuint destFBO, GLuint colorTex, GLuint colorBuf, GLuint depthTex, GLuint depthBuf)
{
#if ENABLE_GLES_WRAPPER
	BOOL isRecording = [everyplayCapture isRecording];

	if (isRecording == NO) {
		return UnityResolveMSAA_Unity(destFBO, colorTex, colorBuf, depthTex, depthBuf);
	} else {
		GLuint msaaFramebufferActive = [everyplayCapture msaaFramebuffer:_surface.msaaFramebuffer];

		GLuint framebuffer = 0;
#if UNITY_VERSION == 350
		framebuffer = _surface.framebuffer;
#elif UNITY_VERSION == 400
		framebuffer = _surface.systemFramebuffer;
#endif

#if GL_APPLE_framebuffer_multisample
		if (_surface.msaaSamples > 0 && _supportsMSAA && destFBO!=msaaFramebufferActive && destFBO!=framebuffer)
		{
			Profiler_StartMSAAResolve();

			GLint oldFBO;
			GLES_CHK( glGetIntegerv (GL_FRAMEBUFFER_BINDING_OES, &oldFBO) );

			UNITY_DBG_LOG ("UnityResolveMSAA: samples=%i msaaFBO=%i destFBO=%i colorTex=%i colorRB=%i depthTex=%i depthRB=%i\n", _surface.msaaSamples, msaaFramebufferActive, destFBO, colorTex, colorBuf, depthTex, depthBuf);
			UNITY_DBG_LOG ("  bind dest as DRAW FBO and textures/buffers into it\n");

			GLES_CHK( glBindFramebufferOES (GL_DRAW_FRAMEBUFFER_APPLE, destFBO) );
			if (colorTex)
				GLES_CHK( glFramebufferTexture2DOES( GL_DRAW_FRAMEBUFFER_APPLE, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, colorTex, 0 ) );
			else if (colorBuf)
				GLES_CHK( glFramebufferRenderbufferOES (GL_DRAW_FRAMEBUFFER_APPLE, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, colorBuf) );

			if (depthTex)
				GLES_CHK( glFramebufferTexture2DOES( GL_DRAW_FRAMEBUFFER_APPLE, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, depthTex, 0 ) );
			else if (depthBuf)
				GLES_CHK( glFramebufferRenderbufferOES (GL_DRAW_FRAMEBUFFER_APPLE, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, depthBuf) );

			UNITY_DBG_LOG ("  bind msaa as READ FBO\n");
			GLES_CHK( glBindFramebufferOES(GL_READ_FRAMEBUFFER_APPLE, msaaFramebufferActive) );

			UNITY_DBG_LOG ("  glResolveMultisampleFramebufferAPPLE ();\n");
			GLES_CHK( glResolveMultisampleFramebufferAPPLE() );

			GLES_CHK( glBindFramebufferOES (GL_FRAMEBUFFER_OES, oldFBO) );

			Profiler_EndMSAAResolve();
			return true;
		}
#endif
	}
	return false;
#else
	return UnityResolveMSAA_Unity(destFBO, colorTex, colorBuf, depthTex, depthBuf);
#endif
}

#else

void CreateUnityRenderBuffers(UnityRenderingSurface* surface)
{
	CreateUnityRenderBuffers_Unity(surface);
#if ENABLE_GLES_WRAPPER
	if (!everyplayCapture) {
		everyplayCapture = [[EveryplayCapture alloc] initWithView:UnityGetGLView()
													  eaglContext:surface->context
														    layer:surface->layer];
		[everyplayCapture setActiveFramebufferCallback:^(GLuint activeFramebuffer) {
			gDefaultFBO = activeFramebuffer;
		}];
	}

	GLuint drawFB = surface->targetFB ? surface->targetFB : surface->systemFB;
	[everyplayCapture createFramebuffer:drawFB withMSAA:surface->msaaFB];
#endif
}

void DestroySystemRenderingSurface(UnityRenderingSurface* surface)
{
#if ENABLE_GLES_WRAPPER
	[everyplayCapture deleteFramebuffer];
#endif
	DestroySystemRenderingSurface_Unity(surface);
}

void PreparePresentRenderingSurface(UnityRenderingSurface* surface, EAGLContext* mainContext)
{
#if ENABLE_GLES_WRAPPER
	BOOL isRecording = [everyplayCapture isRecording];

	{
		EAGLContextSetCurrentAutoRestore autorestore(surface->context);

	#if GL_APPLE_framebuffer_multisample
		if(!isRecording && surface->msaaSamples > 1 && _supportsMSAA)
		{
			Profiler_StartMSAAResolve();

			GLuint targetFB = surface->targetFB ? surface->targetFB : surface->systemFB;

			GLES_CHK(glBindFramebufferOES(GL_READ_FRAMEBUFFER_APPLE, surface->msaaFB));
			GLES_CHK(glBindFramebufferOES(GL_DRAW_FRAMEBUFFER_APPLE, targetFB));
			GLES_CHK(glResolveMultisampleFramebufferAPPLE());

			Profiler_EndMSAAResolve();
		}
	#endif

		if(surface->allowScreenshot && UnityIsCaptureScreenshotRequested())
		{
			GLint targetFB = surface->targetFB ? surface->targetFB : surface->systemFB;
			GLES_CHK(glBindFramebufferOES(GL_FRAMEBUFFER_OES, targetFB));
			UnityCaptureScreenshot();
		}
	}

	if(surface->targetColorRT)
	{
		// shaders are bound to context
		EAGLContextSetCurrentAutoRestore autorestore(mainContext);
		[everyplayCapture beforePresentRenderbuffer:surface->targetFB];

		gDefaultFBO = surface->systemFB;
		GLES_CHK(glBindFramebufferOES(GL_FRAMEBUFFER_OES, gDefaultFBO));
		UnityBlitToSystemFB(surface->targetColorRT, surface->targetW, surface->targetH, surface->systemW, surface->systemH);
	} else {
		[everyplayCapture beforePresentRenderbuffer:surface->systemFB];
	}

#if GL_EXT_discard_framebuffer
	if(!isRecording && _supportsDiscard)
	{
		EAGLContextSetCurrentAutoRestore autorestore(surface->context);

		GLenum	discardAttach[] = {GL_COLOR_ATTACHMENT0_OES, GL_DEPTH_ATTACHMENT_OES, GL_STENCIL_ATTACHMENT_OES};
		GLuint msaaFramebufferActive = [everyplayCapture msaaFramebuffer:surface->msaaFB];

		if(surface->msaaFB && surface->msaaFB == msaaFramebufferActive)
			GLES_CHK(glDiscardFramebufferEXT(GL_READ_FRAMEBUFFER_APPLE, 3, discardAttach));

		if(surface->targetFB)
		{
			GLES_CHK(glBindFramebufferOES(GL_FRAMEBUFFER_OES, surface->targetFB));
			GLES_CHK(glDiscardFramebufferEXT(GL_FRAMEBUFFER_OES, 3, discardAttach));
		}

		GLES_CHK(glBindFramebufferOES(GL_FRAMEBUFFER_OES, surface->systemFB));
		GLES_CHK(glDiscardFramebufferEXT(GL_FRAMEBUFFER_OES, 2, &discardAttach[1]));
	}
#endif
#else
	PreparePresentRenderingSurface_Unity(surface, mainContext);
#endif
}

void SetupUnityDefaultFBO(UnityRenderingSurface* surface)
{
	SetupUnityDefaultFBO_Unity(surface);
#if ENABLE_GLES_WRAPPER
	if (_supportsMSAA && surface->msaaSamples > 1) {
		[everyplayCapture afterPresentRenderbuffer:surface->msaaFB];
	} else {
		[everyplayCapture afterPresentRenderbuffer];
	}
#endif
}

extern "C" bool UnityResolveMSAA(GLuint destFBO, GLuint colorTex, GLuint colorBuf, GLuint depthTex, GLuint depthBuf)
{
#if ENABLE_GLES_WRAPPER
	BOOL isRecording = [everyplayCapture isRecording];

	if (isRecording == NO) {
		return UnityResolveMSAA_Unity(destFBO, colorTex, colorBuf, depthTex, depthBuf);
	} else {
		#if GL_APPLE_framebuffer_multisample

		// TODO: well - only mainScreen for now
		extern const UnityRenderingSurface* UnityDisplayManager_MainDisplayRenderingSurface();
		const UnityRenderingSurface& targetSurface = *UnityDisplayManager_MainDisplayRenderingSurface();

		GLuint msaaFramebufferActive = [everyplayCapture msaaFramebuffer:targetSurface.msaaFB];

		if (targetSurface.msaaSamples > 1 && _supportsMSAA && destFBO!=msaaFramebufferActive && destFBO!=targetSurface.systemFB)
		{
			Profiler_StartMSAAResolve();

			GLint oldFBO;
			GLES_CHK( glGetIntegerv (GL_FRAMEBUFFER_BINDING_OES, &oldFBO) );

			UNITY_DBG_LOG ("UnityResolveMSAA: samples=%i msaaFBO=%i destFBO=%i colorTex=%i colorRB=%i depthTex=%i depthRB=%i\n", targetSurface.msaaSamples, msaaFramebufferActive, destFBO, colorTex, colorBuf, depthTex, depthBuf);
			UNITY_DBG_LOG ("  bind dest as DRAW FBO and textures/buffers into it\n");

			GLES_CHK( glBindFramebufferOES (GL_DRAW_FRAMEBUFFER_APPLE, destFBO) );
			if (colorTex)
				GLES_CHK( glFramebufferTexture2DOES( GL_DRAW_FRAMEBUFFER_APPLE, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, colorTex, 0 ) );
			else if (colorBuf)
				GLES_CHK( glFramebufferRenderbufferOES (GL_DRAW_FRAMEBUFFER_APPLE, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, colorBuf) );

			if (depthTex)
				GLES_CHK( glFramebufferTexture2DOES( GL_DRAW_FRAMEBUFFER_APPLE, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, depthTex, 0 ) );
			else if (depthBuf)
				GLES_CHK( glFramebufferRenderbufferOES (GL_DRAW_FRAMEBUFFER_APPLE, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, depthBuf) );

#if UNITY_VERSION > 410
#if GL_OES_packed_depth_stencil
			// Set stencil attachment to none (we don't care about actually
			// "resolving" stencil). But need to prevent cases where some
			// old attachment was there of different size.
			if (_supportsPackedStencil)
				GLES_CHK( glFramebufferRenderbufferOES( GL_DRAW_FRAMEBUFFER_APPLE, GL_STENCIL_ATTACHMENT, GL_RENDERBUFFER, 0 ) );
#endif
#endif

			UNITY_DBG_LOG ("  bind msaa as READ FBO\n");
			GLES_CHK( glBindFramebufferOES(GL_READ_FRAMEBUFFER_APPLE, msaaFramebufferActive) );

			UNITY_DBG_LOG ("  glResolveMultisampleFramebufferAPPLE ();\n");
			GLES_CHK( glResolveMultisampleFramebufferAPPLE() );

			GLES_CHK( glBindFramebufferOES (GL_FRAMEBUFFER_OES, oldFBO) );

			Profiler_EndMSAAResolve();
			return true;
		}
		#endif
	return false;
    }
#else
	return UnityResolveMSAA_Unity(destFBO, colorTex, colorBuf, depthTex, depthBuf);
#endif
}

#endif

#endif
