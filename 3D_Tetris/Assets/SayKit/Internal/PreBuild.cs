#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
using System.Linq;
using SayKitInternal;

public class ApplicationSettings
{
    public static string facebook_app_id;
    public static string facebook_app_name;
}

public class SayKitPreBuild : IPreprocessBuildWithReport
{
    const string DIALOG_TITLE = "SayKit Build";

    public bool dirtyFlag = false;

    public int callbackOrder { get { return 0; } }

    public static string buildPlace = "default";



    public void OnPreprocessBuild(BuildReport report)
    {
        OnPreprocessBuild(report, false);
    }

    interface IPreBuildTask
    {
        string Title();
        string Run();
    }

    class EmbedConfig : IPreBuildTask
    {
        public string Title() { return "Embedding config"; }
        public string Run()
        {
            string result = "";

            string path = "Assets/Resources";

#if UNITY_IOS
            string os = "ios";
#else
            string os = "android";
#endif

            string key = path + "/saykit_" + Application.version + ".json";
            string url = "https://app.saygames.io/hero/config?bundle=" + Application.identifier + "&os=" + os + "&_=" + UnityEngine.Random.Range(100000000, 900000000);

            string errorTemplate = "Can't embed config for " + Application.identifier + "\n\nUrl: " + url + "\n\nLocal path: " + key + "\n\n";


            SayKitWebRequest sayKitWebRequest = new SayKitWebRequest(url);
            sayKitWebRequest.SendAndWait(10);

            string data = sayKitWebRequest.Text;
            if (data.Length > 0 && data[0] == '{')
            {
                try
                {
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    System.IO.File.WriteAllText(key, data);
                    UnityEditor.AssetDatabase.Refresh();

                    var config = JsonUtility.FromJson<RemoteConfig>(data);

                    if (config.ads_settings.saymed_enabled != 1
                        || String.IsNullOrEmpty(config.ads_settings.saymed_interstitial_id)
                        || String.IsNullOrEmpty(config.ads_settings.saymed_rewarded_id))
                    {
                        result = "SayMediation is not configured, please connect with SayGames support team.";
                    }

                }
                catch (System.Exception e)
                {
                    result = errorTemplate + "Error in saving. " + e;
                }
            }
            else
            {
                result = errorTemplate + "Error in downloading. " + sayKitWebRequest.ErrorMessage;
            }

            return result;
        }
    }

    class CheckRemoteConfigs : IPreBuildTask
    {
        [Serializable]
        public class SayKitRemoteData
        {
            public string[] Errors;
            public string[] Messages;

            public string FacebookAppId;
            public string FacebookAppName;
            public string AdjustToken;
        }

        public string Title() { return "Checking remote configurations"; }
        public string Run()
        {
            return DownloadConfig();
        }


        public string GetRemoteURL()
        {
            string url = "https://hero.say.games/saykit/configure?";
        
#if UNITY_IOS
            url += "os=" + "ios";
#elif UNITY_ANDROID
            url += "os=" + "android";
#endif

            url += "&bundle=" + Application.identifier;

#if UNITY_IOS
            url += "&secret=" + SayKitKey.SECRET_IOS;
#elif UNITY_ANDROID
        url += "&secret=" + SayKitKey.SECRET_ANDROID;
#endif

            url += "&saykit_version=" + SayKit.GetVersion;
            url += "&unity_version=" + Application.unityVersion;



            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                url += "&unity_os=" + "mac";
            }
            else
            {
                url += "&unity_os=" + "pc";
            }


            return url;
        }

        
        public string DownloadConfig()
        {
            string url = GetRemoteURL();

            SayKitWebRequest sayKitWebRequest = new SayKitWebRequest(url);
            sayKitWebRequest.SendAndWait(10);

            try
            {
                if (sayKitWebRequest.IsDone && string.IsNullOrEmpty(sayKitWebRequest.ErrorMessage))
                {
                    if (sayKitWebRequest.Text.Length > 0 && sayKitWebRequest.Text[0] == '{')
                    {
                        var data = sayKitWebRequest.Text;

                        string path = "Assets/Resources";
                        string sayKitAttributionSettingsFilePath = path + "/saykit_attribution_settings.json";

                        var attributionData = new AttributionData();
                        var config = JsonUtility.FromJson<SayKitRemoteData>(data);



                        if (config.Errors?.Length == 0)
                        {
                            if (config.Messages?.Length > 0)
                            {
                                for (int i = 0; i < config.Messages.Length; i++)
                                {
                                    EditorUtility.DisplayDialog("SayKit Message", config.Messages[i], "OK");
                                    Debug.Log("SayKit: " + config.Messages[i]);
                                }
                            }

                            ApplicationSettings.facebook_app_id = config.FacebookAppId;
                            ApplicationSettings.facebook_app_name = config.FacebookAppName;

                            if (String.IsNullOrEmpty(ApplicationSettings.facebook_app_id)
                                || String.IsNullOrEmpty(ApplicationSettings.facebook_app_name))
                            {
                                return "Facebook is not configurated. Please, check your internet connection or connect to SayGames support team.";
                            }
                              

                            attributionData.AttributionToken = config.AdjustToken;

                            if(attributionData.AttributionToken == null 
                                || attributionData.AttributionToken =="")
                            {
                                return "Adjust is not configurated. Please, check your internet connection or connect to SayGames support team.";
                            }
                            else {
                                var attributionJSON = JsonUtility.ToJson(attributionData);
                                File.WriteAllText(sayKitAttributionSettingsFilePath, attributionJSON);
                            }

                            return "";
                        }
                        else
                        {
                            if (config.Errors?.Length > 0)
                            {
                                for (int i = 0; i < config.Errors.Length; i++)
                                {
                                    Debug.LogError("Error when loading google settings: " + config.Errors[i]);
                                }

                                return "SayKit settings wasn't loaded from server. Please, check the logs for more information.";
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Error when loading google settings, content data is not correct: " + sayKitWebRequest.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception when loading google settings: " + ex.Message);
            }

            return "SayKit settings wasn't loaded from server. Please, check the logs for more information.";
        }
    }

    class CreateDependentFolders : IPreBuildTask
    {

        public string Title() { return "Create dependent folders."; }
        public string Run()
        {

            string pluginPath = "Assets/Plugins";
            if (!System.IO.Directory.Exists(pluginPath))
            {
                System.IO.Directory.CreateDirectory(pluginPath);
            }

            string iOSPath = "Assets/Plugins/iOS";
            if (!System.IO.Directory.Exists(iOSPath))
            {
                System.IO.Directory.CreateDirectory(iOSPath);
            }

            string androidPath = "Assets/Plugins/Android";
            if (!System.IO.Directory.Exists(androidPath))
            {
                System.IO.Directory.CreateDirectory(androidPath);
            }


            string sayKitPath = "Assets/Plugins/Android/saykit";
            if (!System.IO.Directory.Exists(sayKitPath))
            {
                System.IO.Directory.CreateDirectory(sayKitPath);
            }


            string resPath = "Assets/Plugins/Android/saykit/res";
            if (!System.IO.Directory.Exists(resPath))
            {
                System.IO.Directory.CreateDirectory(resPath);
            }


            string valuesPath = "Assets/Plugins/Android/saykit/res/values";
            if (!System.IO.Directory.Exists(valuesPath))
            {
                System.IO.Directory.CreateDirectory(valuesPath);
            }

            string xmlPath = "Assets/Plugins/Android/saykit/res/xml";
            if (!System.IO.Directory.Exists(xmlPath))
            {
                System.IO.Directory.CreateDirectory(xmlPath);
            }

            return "";
        }
    }

   
    class CheckBuildSettings : IPreBuildTask
    {

        public string Title() { return "Checking build settings"; }
        public string Run()
        {
#if UNITY_IOS

#elif UNITY_ANDROID
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP)
            {
                return "Set IL2CPP as Scripting Backend\n\nGo to Player Settings -> Other Settings -> Configuration";
            }
#endif

            return "";
        }

    }

    class CheckMultiDexFabricApplicationSettings : IPreBuildTask
    {

        public string Title() { return "Checking MultiDexFabricApplication settings"; }
        public string Run()
        {

#if UNITY_ANDROID
            string path = "Assets/Plugins/Android/";
            string manifestPath = Path.Combine(path, "AndroidManifest.xml");

            if (System.IO.File.Exists(manifestPath))
            {
                string str = File.ReadAllText(manifestPath);
                if (str.Contains("MultiDexFabricApplication"))
                {
                    return "AndroidManifest.xml contains MultiDexFabricApplication settings. Please, delete android:name=\"io.fabric.unity.android.MultiDexFabricApplication\" line from Assets/Plugins/Android/AndroidManifest.xml file.";
                }
            }
#endif

            return "";
        }

    }



    class ConfigurateGenerateFiles : IPreBuildTask
    {
        private string _settingsPath = "Assets/SayKit/Internal/Plugins/Settings/";

        public string Title() { return "Configurate gradle and manifest files."; }
        public string Run()
        {

#if UNITY_ANDROID

#if UNITY_2020_2_OR_NEWER
            var gradleResult = CheckGradleFile("mainTemplate2020.gradle", "mainTemplate.gradle");
            var baseGradleResult = CheckGradleFile("baseProjectTemplate2020.gradle", "baseProjectTemplate.gradle");
            var gradleTemplateResult = CheckGradleFile("gradleTemplate2020.properties", "gradleTemplate.properties");
            var launcherTemplate = CheckGradleFile("launcherTemplate2020.gradle", "launcherTemplate.gradle");


            if (baseGradleResult.Length > 0)
            {
                return baseGradleResult;
            }

            if (gradleTemplateResult.Length > 0)
            {
                return gradleTemplateResult;
            }

            if (launcherTemplate.Length > 0)
            {
                return launcherTemplate;
            }

#elif UNITY_2019_3_OR_NEWER

            var gradleResult = CheckGradleFile("mainTemplate2019.gradle", "mainTemplate.gradle");
            var baseGradleResult = CheckGradleFile("baseProjectTemplate2019.gradle", "baseProjectTemplate.gradle");
            var gradleTemplateResult = CheckGradleFile("gradleTemplate2019.properties", "gradleTemplate.properties");
            var launcherTemplate = CheckGradleFile("launcherTemplate2019.gradle", "launcherTemplate.gradle");
            

            if (baseGradleResult.Length > 0)
            {
                return baseGradleResult;
            }

            if (gradleTemplateResult.Length > 0)
            {
                return gradleTemplateResult;
            }

            if (launcherTemplate.Length > 0)
            {
                return launcherTemplate;
            }
#else
            var gradleResult = CheckGradleFile("mainTemplate.gradle", "mainTemplate.gradle");
#endif

            var manifestResult = CheckManifestFile();
            var sayKitResult = CheckSayKitInitialize();
            var networkSecurityResult = CheckNetworkSecurityConfigFile();
            var valuesSettingsResult = CheckValuesSettings();

            if (gradleResult.Length > 0)
            {
                return gradleResult;
            }
            if (manifestResult.Length > 0)
            {
                return manifestResult;
            }
            if (sayKitResult.Length > 0)
            {
                return sayKitResult;
            }
            if (networkSecurityResult.Length > 0)
            {
                return networkSecurityResult;
            }
            if (valuesSettingsResult.Length > 0)
            {
                return valuesSettingsResult;
            }

#endif

            AssetDatabase.Refresh();

            return "";
        }

        public string CheckGradleFile(string gradleName, string gradleDestinationName)
        {
            string destinationPath = "Assets/Plugins/Android/";

            string gradlePath = Path.Combine(_settingsPath, gradleName);

            string gradleDestinationPath = Path.Combine(destinationPath, gradleDestinationName);

            return CheckFile(gradlePath, gradleDestinationPath, gradleDestinationName);
        }

        public string CheckManifestFile()
        {
            string destinationPath = "Assets/Plugins/Android/";

                        
#if UNITY_2020_2_OR_NEWER
            string manifestPath = Path.Combine(_settingsPath, "AndroidManifest2020.xml");
#else
            string manifestPath = Path.Combine(_settingsPath, "AndroidManifest.xml");
#endif

            string manifestDestinationPath = Path.Combine(destinationPath, "AndroidManifest.xml");

            return CheckFile(manifestPath, manifestDestinationPath, "AndroidManifest.xml");
        }

        public string CheckSayKitInitialize()
        {
            var directoryPath = "Assets/Plugins/Android/saykit";
            CheckDirectory(directoryPath);


            string projectPropertiesPath = Path.Combine(_settingsPath + "saykit/", "project.properties");
            string projectPropertiesDestinationPath = Path.Combine(directoryPath + "/", "project.properties");

            if (CheckFile(projectPropertiesPath, projectPropertiesDestinationPath, "project.properties").Length > 0)
            {
                var lines = new List<string>
                    {
                        "android.library=true"
                    };

                File.AppendAllLines(projectPropertiesDestinationPath, lines);
            }


            string manifestPath = Path.Combine(_settingsPath + "saykit/", "AndroidManifest.xml");
            string manifestDestinationPath = Path.Combine(directoryPath + "/", "AndroidManifest.xml");

            return CheckFile(manifestPath, manifestDestinationPath, "AndroidManifest.xml");
        }

        public string CheckNetworkSecurityConfigFile()
        {
            var resDirectoryPath = "Assets/Plugins/Android/saykit/res";

            CheckDirectory(resDirectoryPath);

            var xmlDirectoryPath = "Assets/Plugins/Android/saykit/res/xml";

            CheckDirectory(xmlDirectoryPath);


            string networkSecurityConfigFilePath = Path.Combine(_settingsPath + "saykit/res/xml/", "network_security_config.xml");
            string networkSecurityConfigDestinationPath = Path.Combine(xmlDirectoryPath + "/", "network_security_config.xml");

            return CheckFile(networkSecurityConfigFilePath, networkSecurityConfigDestinationPath, "network_security_config.xml");
        }


        public string CheckValuesSettings()
        {
            if (String.IsNullOrEmpty(ApplicationSettings.facebook_app_name)
                || String.IsNullOrEmpty(ApplicationSettings.facebook_app_id))
            {
                return "facebook_app_name, facebook_app_id did't download from server. Please, check your internet connection or connect to SayGames support team.";
            }
            else
            {

                var stringsPath = "Assets/Plugins/Android/saykit/res/values/strings.xml";
                var lines = new List<string>
                    {
                        "<resources>",
                        "<string name=\"app_name\">" + ApplicationSettings.facebook_app_name + "</string>",
                        "<string name=\"facebook_app_id\">" + ApplicationSettings.facebook_app_id + "</string>",
                        "<string name=\"fb_login_protocol_scheme\">fb" + ApplicationSettings.facebook_app_id + "</string>",
                        "</resources>"
                    };

                if (!System.IO.File.Exists(stringsPath))
                {
                    File.WriteAllLines(stringsPath, lines);
                }
                else
                {

#if !SAYKIT_AUTOFIX_DISABLE
                    File.WriteAllLines(stringsPath, lines);
#endif

                    string[] baseFileLines = File.ReadAllLines(stringsPath);

                    foreach (var item in baseFileLines)
                    {
                        if (item.Length > 0 && !lines.Any(t => t.Replace(" ", "") == item.Replace(" ", "")))
                        {
                            return "facebook_app_name, facebook_app_id aren't configurated correctrly. Please, check a Assets/Plugins/Android/saykit/res/values/strings.xml file.";
                        }
                    }
                }

            }

            return "";
        }


        public void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string CheckFile(string baseFile, string targetFile, string fileName)
        {
            if (!System.IO.File.Exists(targetFile))
            {
                File.Copy(baseFile, targetFile, true);
            }
            else
            {

#if !SAYKIT_AUTOFIX_DISABLE
                File.Copy(baseFile, targetFile, true);
#endif
                if (!CompareFiles(baseFile, targetFile))
                {
                    return fileName + " file doesn't contain depended lines. " +
                        "Please, check a " + targetFile + " file." +
                        " It needs to contain all data from a " + baseFile + " file."
                        + "\nYou can delete " + targetFile + " and it will be generated correctly.";
                }
            }

            return "";
        }

        public bool CompareFiles(string baseFile, string targetFile)
        {
            int i = 0;
            string[] baseFileLines = File.ReadAllLines(baseFile);
            string[] targetFileLines = File.ReadAllLines(targetFile);

            foreach (var item in baseFileLines)
            {
                i++;

                var line = item.Replace(" ", "").Replace("\t", ""); ;
                if (line.Length > 0 && !targetFileLines.Any(t => t.Replace(" ", "").Replace("\t", "") == line))
                {
                    Debug.Log("Cannot find |" + line + "| line number " + i + " in a " + targetFile + " file. Please, compare data from it with a " + baseFile + " file.");
                    return false;
                }
            }

            return true;
        }

    }




    public class AndroidLibObject
    {
        public AndroidLibObject(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name;
        public string Version;
        public bool IsConfigurated;
    }

    public class AndroidVersionList
    {

        public List<AndroidLibObject> GradleLibsList;
        public List<AndroidLibObject> JarLibsList;
        public List<AndroidLibObject> AarLibsList;

        public AndroidVersionList()
        {
            /* Gradle ******* */

            GradleLibsList = new List<AndroidLibObject>
            {
                new AndroidLibObject("facebook-android-sdk", "13.0.0"),
                new AndroidLibObject("applovin-sdk", "11.3.1"),
            };

        }

    }


    class CheckAndroidSettings : IPreBuildTask
    {

        public string Title() { return "Checking gradle settings"; }

        public string CutGradleLibVersion(string str)
        {
            if (str.Contains("@aar"))
            {
                return CutGradleLibVersion(str, 5);
            }
            else
            {
                return CutGradleLibVersion(str, 1);
            }
        }

        private string CutGradleLibVersion(string str, int tailLength)
        {
            var splits = str.Split(':');
            var split = splits[splits.Length - 1];

            return split.Substring(0, split.Length - tailLength);
        }

        public string CutLibVersion(string str)
        {
            return CutLibVersion(str, 4);
        }

        public string CutGradleJarVersion(string str)
        {
            return CutLibVersion(str, 6);
        }

        public string CutGradleAarVersion(string str)
        {
            if (str.Contains("@aar"))
            {
                return CutGradleLibVersion(str, 5);
            }
            else
            {
                var splits = str.Split(',');
                var split = splits[0];
                return CutLibVersion(split, 1);
            }
        }

        private string CutLibVersion(string str, int tailLength)
        {
            var splits = str.Split('-');
            var split = splits[splits.Length - 1];

            return split.Substring(0, split.Length - tailLength);
        }

        public bool CheckVersionList(List<AndroidLibObject> versionList)
        {
            bool isAllLibsInitialized = true;
            foreach (var lib in versionList)
            {
                if (!lib.IsConfigurated)
                {
                    isAllLibsInitialized = false;
                    Debug.LogError("Error: " + " Library: " + lib.Name + " " + lib.Version + "  is not found.");
                }
            }

            return isAllLibsInitialized;
        }

        public void CheckLibsDirectory(List<AndroidLibObject> versionList, string type)
        {
            string path = "Assets/SayKit/Internal/Plugins/Android";

            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles("*." + type);

            foreach (FileInfo file in Files)
            {
                foreach (var lib in versionList)
                {
                    if (file.Name.Contains(lib.Name))
                    {
                        var version = CutLibVersion(file.Name);
                        if (lib.Version == "" || lib.Version.Equals(version))
                        {
                            lib.IsConfigurated = true;
                        }
                    }
                }
            }
        }

        public void CheckGradleFile(AndroidVersionList versionList)
        {
            string path = "Assets/Plugins/Android/";
            string gradlePath = Path.Combine(path, "mainTemplate.gradle");

            if (System.IO.File.Exists(gradlePath))
            {
                string str = File.ReadAllText(gradlePath);
                string[] lines = File.ReadAllLines(gradlePath);

                foreach (var line in lines)
                {
                    if (line.Contains("implementation"))
                    {

                        foreach (var lib in versionList.GradleLibsList)
                        {
                            if (line.Contains(lib.Name))
                            {
                                var version = CutGradleLibVersion(line);
                                if (lib.Version.Equals(version))
                                {
                                    lib.IsConfigurated = true;
                                }
                            }
                        }
                    }
                    
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public string CheckMinAPILevel()
        {
            string errorMessage = "";

          


#if UNITY_2019_4_OR_NEWER
       
#endif

#if UNITY_2019_3_OR_NEWER

#endif

            return errorMessage;
        }


        public string Run()
        {
            AndroidVersionList versionList = new AndroidVersionList();

            CheckGradleFile(versionList);

            if (!CheckVersionList(versionList.GradleLibsList))
            {
                return "Android libraries is not configured correctly. Please check the logs for more information.";
            }

            return CheckMinAPILevel();
        }
    }


    public void OnPreprocessBuild(BuildReport report, bool releaseCheck)
    {

        int tasksNumber = 3;

        dirtyFlag = false;

        var tasks = new List<IPreBuildTask>();

        tasks.Add(new CreateDependentFolders());

        // Embed Config
#if UNITY_IOS
        tasks.Add(new EmbedConfig());

        tasks.Add(new CheckRemoteConfigs());
        tasks.Add(new ConfigurateGenerateFiles());

#elif UNITY_ANDROID

#if UNITY_2019_3_OR_NEWER
        CheckKyestoreFile();

        //unity 2019.3 export file bug
        // File.WriteAllText($"{report.summary.outputPath}/build.gradle.NEW", "");
#endif
        tasks.Add(new EmbedConfig());

        tasks.Add(new CheckRemoteConfigs());
        tasks.Add(new ConfigurateGenerateFiles());

        tasks.Add(new CheckAndroidSettings());
        tasks.Add(new CheckMultiDexFabricApplicationSettings());
#endif

        // Check platform settings
        if (releaseCheck)
        {
            tasks.Add(new CheckBuildSettings());
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            EditorUtility.DisplayProgressBar(DIALOG_TITLE, tasks[i].Title(), (float)i / tasksNumber);
            this.Assert(tasks[i].Run());
        }


        if (Application.isBatchMode) {
            if (dirtyFlag)
            {
                EditorApplication.Exit(1);
            }
        }

        // We are done.
        EditorUtility.ClearProgressBar();
    }

    public void Assert(string result)
    {
        if (result.Length > 0)
        {
            dirtyFlag = true;
            Debug.LogError("SayKit: " + result);

            if (!Application.isBatchMode) {
                EditorUtility.DisplayDialog(DIALOG_TITLE, result, "OK");
            }
        }
    }

    public void CheckKyestoreFile()
    {
        string applicationPath = Application.dataPath.Replace("/Assets", "");
        DirectoryInfo dirInfo = new DirectoryInfo(applicationPath);

        var applicationFiles = dirInfo.GetFiles();

        for (int i = 0; i < applicationFiles.Length; i++)
        {
            var fileName = applicationFiles[i].Name;
            if (fileName.Contains("keystore"))
            {
                PlayerSettings.Android.keystoreName = Path.GetFullPath(fileName);
                return;
            }
        }
    }

   
}
#endif