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

#import <Foundation/Foundation.h>
#import <Everyplay/Everyplay.h>

#if UNITY_VERSION >= 430
#import "AppDelegateListener.h"
@interface EveryplayUnity : NSObject<EveryplayDelegate, AppDelegateListener> {
#else
    @interface EveryplayUnity : NSObject<EveryplayDelegate> {
#endif
    BOOL displayLinkPaused;
    UIInterfaceOrientation currentOrientation;
}

+ (EveryplayUnity *) sharedInstance;

@end

// This isn't defined by default for Unity generated Xcode projects
//#define DEBUG 1

// Conditional debug
#if DEBUG
#define EveryplayLog(fmt, ...) NSLog((@"[#%.3d] %s " fmt), __LINE__, __PRETTY_FUNCTION__, ## __VA_ARGS__)
#else
#define EveryplayLog(...)
#endif

    // ApplifierALog always displays output regardless of the DEBUG setting
#ifndef EveryplayALog
#define EveryplayALog(fmt, ...)   NSLog((@"[#%.3d] %s " fmt), __LINE__, __PRETTY_FUNCTION__, ## __VA_ARGS__)
#endif

#define ELOG EveryplayLog(@"")
