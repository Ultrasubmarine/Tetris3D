#if UNITY_EDITOR && UNITY_IOS
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;

using UnityEditor.iOS.Xcode;
using System.Text;
using SayKitInternal;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif


public class SayKitPostBuild {

    [PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject) {
		
       if (target != BuildTarget.iOS)
            return;

       Debug.Log("SayKit: Updating Info.plist");

#if UNITY_2018_3_3
		var frameworksPath = "Frameworks/";
#else
		var frameworksPath = "Frameworks/SayKit/Internal/Plugins/iOS/";
#endif

        string plistPath = Path.Combine(pathToBuildProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        //Get Root
        PlistElementDict rootDict = plist.root;

        //Add Queries Schemes
        PlistElementArray LSApplicationQueriesSchemes = rootDict.CreateArray("LSApplicationQueriesSchemes");
        LSApplicationQueriesSchemes.AddString("fb");
        LSApplicationQueriesSchemes.AddString("instagram");
        LSApplicationQueriesSchemes.AddString("tumblr");
        LSApplicationQueriesSchemes.AddString("twitter");

        List<string> queriesSchemes = new List<string>();
        queriesSchemes.Add("fb412266819304521");
        queriesSchemes.Add("fb2326383180965488");
        for (int i = 1; i <= 40; i++)
        {
            queriesSchemes.Add("saygames" + i);
        }


        // Register URL Schemes
        List<string> urlSchemes = new List<string>();
        urlSchemes.Add("fb" + SayKitRemoteSettings.SharedInstance.facebook_app_id);
        

        PlistElementArray CFBundleURLTypes = rootDict.CreateArray("CFBundleURLTypes");

        foreach (var scheme in urlSchemes)
        {
            PlistElementDict dict = CFBundleURLTypes.AddDict();
            PlistElementArray array = new PlistElementArray();
            array.AddString(scheme);
            dict["CFBundleURLSchemes"] = array;
        }


        rootDict.SetString("FacebookAppID", SayKitRemoteSettings.SharedInstance.facebook_app_id);
        rootDict.SetString("FacebookDisplayName", SayKitRemoteSettings.SharedInstance.facebook_app_name);

        PlistElementDict NSAppTransportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
        NSAppTransportSecurity.SetBoolean("NSAllowsArbitraryLoads", true);


        rootDict.SetString("NSCameraUsageDescription", "This app does not use the camera.");
        rootDict.SetString("NSCalendarsUsageDescription", "This app does not use the calendar.");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "This app does not use the photo library.");
        rootDict.SetString("NSMotionUsageDescription", "This app does not use the accelerometer.");
		rootDict.SetString("NSLocationAlwaysUsageDescription", "This app does not use the location.");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "This app does not use the location.");
        rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "This app does not use the location.");

        //rootDict.SetString("NSUserTrackingUsageDescription", "This will only be used to serve more relevant ads.");


        rootDict.SetBoolean("GADIsAdManagerApp", true);
		rootDict.SetBoolean("FacebookAdvertiserIDCollectionEnabled", true);
		rootDict.SetBoolean("FacebookAutoLogAppEventsEnabled", true);


        // Remove "UIApplicationExitsOnSuspend" flag.
        string exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
        if (rootDict.values.ContainsKey(exitsOnSuspendKey))
        {
            rootDict.values.Remove(exitsOnSuspendKey);
        }

        InsertSkAdNetworkIds(rootDict);



        File.WriteAllText (plistPath, plist.WriteToString ());

		// XCode project
		Debug.Log("SayKit: Updating Xcode project");


        var projPath = Path.Combine(pathToBuildProject, "Unity-iPhone.xcodeproj/project.pbxproj");
        var project = new PBXProject();
       
        project.ReadFromString(File.ReadAllText(projPath));

#if UNITY_2019_3_OR_NEWER
        var projTarget = project.GetUnityFrameworkTargetGuid();
        var mainTarget = project.GetUnityMainTargetGuid();

        project.AddBuildProperty(mainTarget, "EMBEDDED_CONTENT_CONTAINS_SWIFT", "NO");
        project.AddBuildProperty(projTarget, "EMBEDDED_CONTENT_CONTAINS_SWIFT", "NO");
#else
        var projTarget = project.TargetGuidByName("Unity-iPhone");
        project.AddBuildProperty(projTarget, "EMBEDDED_CONTENT_CONTAINS_SWIFT", "YES");
#endif

        project.SetBuildProperty(
            projTarget, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");

        project.AddBuildProperty(projTarget, "OTHER_LDFLAGS", "-ObjC");
        project.AddBuildProperty(projTarget, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
        project.AddBuildProperty(projTarget, "CLANG_ENABLE_MODULES", "YES");
        project.AddBuildProperty(projTarget, "ENABLE_BITCODE", "NO");
        project.AddBuildProperty(projTarget, "SWIFT_VERSION", "5.0");


        // Required Frameworks
        var frameworks = new string[] {
            "Accelerate.framework",
			"AdSupport.framework",
			"AVFoundation.framework",
			"CoreGraphics.framework",
			"CoreLocation.framework",
			"CoreMedia.framework",
			"CoreTelephony.framework",
			"Foundation.framework",
			"MediaPlayer.framework",
			"MessageUI.framework",
			"QuartzCore.framework",
			"SafariServices.framework",
			"StoreKit.framework",
			"SystemConfiguration.framework",
			"UIKit.framework",
			"WebKit.framework",

			"MobileCoreServices.framework",
			"Photos.framework",
            "VideoToolbox.framework",

            //"AppTrackingTransparency.framework"
        };

		foreach (string framework in frameworks) {
			if (!project.ContainsFramework(projTarget, framework)) {
				Debug.Log ("SayKit: Adding " + framework + " to Xcode project");
				project.AddFrameworkToProject (projTarget, framework, false);
			}
		}


#if UNITY_CLOUD_BUILD
        project.RemoveFrameworkFromProject(projTarget, "StoreKit.framework");
#endif

#if UNITY_2019_3_OR_NEWER
        project.AddBuildProperty(mainTarget, "ENABLE_BITCODE", "NO");
        project.AddBuildProperty(projTarget, "ENABLE_BITCODE", "NO");
#endif


#if UNITY_2019_3_OR_NEWER || UNITY_2019_4_OR_NEWER
        CommentRowsInUnityCleanupTrampoline(pathToBuildProject);
#endif


        // Required Libs
        var libs = new string[] {
			"libresolv.9.tbd",
			"libc++.tbd",
			"libz.tbd",
            "libbz2.tbd",

            "libz.dylib",
            "libsqlite3.dylib",
            "libxml2.dylib"
        };

		foreach (string lib in libs) {
        	Debug.Log ("SayKit: Adding " + lib + " to Xcode project");

			string libGuid = project.AddFile("usr/lib/" + lib, "Libraries/" + lib, PBXSourceTree.Sdk);
			project.AddFileToBuild(projTarget, libGuid);
        }


		File.WriteAllText(projPath, project.WriteToString());

        CheckXCodeProjectSettings(target, pathToBuildProject);
        RenameMRAIDSource(pathToBuildProject);
    }

	private static void InsertSkAdNetworkIds(PlistElementDict rootDict)
	{
		string idsPath = Application.dataPath + "/SayKit/Internal/SKAdNetworkItems.json";
		string jsonContent = File.ReadAllText(idsPath);
		Dictionary<string, object> json = Json.Deserialize(jsonContent) as Dictionary<string, object>;
		List<object> networks = json["networks"] as List<object>;
		HashSet<string> allSkIds = new HashSet<string>();
		foreach (Dictionary<string, object> network in networks.Cast<Dictionary<string, object>>())
		{
			List<object> ids = network["items"] as List<object>;
			allSkIds.UnionWith(ids.Cast<string>());
		}

		PlistElementArray array = rootDict.CreateArray("SKAdNetworkItems");
		foreach (string id in allSkIds)
		{
			PlistElementDict pair = array.AddDict();
			pair["SKAdNetworkIdentifier"] = new PlistElementString(id);
		}
	}


	public static void CheckXCodeProjectSettings(BuildTarget target, string pathToBuiltProject)
    {
        var xcodeProjectPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj");
        var pbxPath = Path.Combine(xcodeProjectPath, "project.pbxproj");

        var xcodeProjectLines = File.ReadAllLines(pbxPath);
        var sb = new StringBuilder();
        var isNeedToAddValidArchs = false;

        foreach (var line in xcodeProjectLines)
        {
            if (line.Contains("GCC_ENABLE_OBJC_EXCEPTIONS") ||
                line.Contains("GCC_ENABLE_CPP_EXCEPTIONS") ||
                line.Contains("CLANG_ENABLE_MODULES"))
            {
                var newLine = line.Replace("NO", "YES");
                sb.AppendLine(newLine);

                isNeedToAddValidArchs = true;
            }
            else
            {
                sb.AppendLine(line);
            }

            if (isNeedToAddValidArchs && line.Contains("USYM_UPLOAD_URL_SOURCE"))
            {
                isNeedToAddValidArchs = false;
                sb.AppendLine("VALID_ARCHS = \"arm64 armv7 armv7s\";");
            }
        }

        File.WriteAllText(pbxPath, sb.ToString());
    }


    
	private static readonly string DIALOG_TITLE = "SayKit Xcode Project";

    private static void RenameMRAIDSource(string buildPath)
    {
        // Unity will try to compile anything with the ".js" extension. Since mraid.js is not intended
        // for Unity, it'd break the build. So we store the file with a masked extension and after the
        // build rename it to the correct one.

        var maskedFiles = Directory.GetFiles(
            buildPath, "*.prevent_unity_compilation", SearchOption.AllDirectories);
        foreach (var maskedFile in maskedFiles)
        {
            var unmaskedFile = maskedFile.Replace(".prevent_unity_compilation", "");
            File.Move(maskedFile, unmaskedFile);
        }
    }


#if UNITY_2019_3_OR_NEWER || UNITY_2019_4_OR_NEWER
    private static void CommentRowsInUnityCleanupTrampoline(string pathToBuildProject)
    {
        string filePath = Path.Combine(pathToBuildProject, "Classes/UnityAppController.mm");
        string data = File.ReadAllText(filePath);

        data = data.Replace("[_UnityAppController window].rootViewController = nil;", "//[_UnityAppController window].rootViewController = nil;");
        data = data.Replace("[[_UnityAppController unityView] removeFromSuperview];", "//[[_UnityAppController unityView] removeFromSuperview];");
        
        File.WriteAllText(filePath, data);
    }
#endif

}
#endif
