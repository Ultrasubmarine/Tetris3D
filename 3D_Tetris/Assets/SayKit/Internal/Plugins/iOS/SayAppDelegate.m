//
//  SayAppDelegate.m
//  Unity-iPhone
//
//  Created by Timur Dularidze on 9/25/20.
//

#import "UnityAppController.h"
#import <FBSDKCoreKit/FBSDKCoreKit.h>

@interface SayAppDelegate : UnityAppController
@end

IMPL_APP_CONTROLLER_SUBCLASS(SayAppDelegate)

@implementation SayAppDelegate

-(BOOL)application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) options
{
    // From 9.0.0, developers are required to initialize the SDK explicitly with the initializeSDK method or implicitly by calling it in applicationDidFinishLaunching.
    [[FBSDKApplicationDelegate sharedInstance] application:application
                               didFinishLaunchingWithOptions:options];
    
    return [super application:application didFinishLaunchingWithOptions:options];
}

@end
