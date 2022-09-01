#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;

using UnityEngine;
using UnityEditor.Android;
using System.Linq;


public class PostGenerateGradleAndroidProject : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 0;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        UpdateUnityLibraryGradleFile(path);

        var unityLibraryPath = path;
        path = path.Replace("unityLibrary", "launcher");
        
        UpdateGradleProperties(path);


#if UNITY_2020_2_OR_NEWER
        var stringsXML = Application.dataPath + "/Plugins/Android/saykit/res/values/strings.xml";
        var networkConfigXML = Application.dataPath + "/Plugins/Android/saykit/res/xml/network_security_config.xml";
        var resPath = unityLibraryPath + "/src/main/res";

        Directory.CreateDirectory(resPath + "/xml");
        File.Copy(networkConfigXML, resPath + "/xml/network_security_config.xml", true);
        File.Copy(stringsXML, resPath + "/values/strings.xml", true);
#else
        UpdateSayKitGradleFile(path);
#endif

        UpdateGradleWrapperFile(path);
    }

    public void UpdateGradleProperties(string path)
    {
        path = path.Replace("/launcher", "");
        string gradlePropertiesFile = path + "/gradle.properties";


        StreamWriter writer = File.AppendText(gradlePropertiesFile);
        writer.WriteLine("");
        writer.WriteLine("android.useAndroidX=true");
        writer.WriteLine("android.enableJetifier=true");
        writer.Flush();
        writer.Close();
    }


    public void UpdateUnityLibraryGradleFile(string path)
    {
        string gradleFile = path + "/build.gradle";

        var lines = File.ReadAllLines(gradleFile).ToList();


        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains("'UnityAds'"))
            {
                lines[i] = "";
                break;
            }
        }

        File.WriteAllLines(gradleFile, lines.ToArray());
    }
    
    private void UpdateGradleWrapperFile(string path)
    {
        var projectPath = string.Copy(path);        
        projectPath = projectPath.Replace("/launcher", "");
        
        if (!Directory.Exists(projectPath + "/gradle"))
        {
            Directory.CreateDirectory(projectPath + "/gradle");
        }

        if (!Directory.Exists(projectPath + "/gradle/wrapper"))
        {
            Directory.CreateDirectory(projectPath + "/gradle/wrapper");
        }
        
        var gradleWrapperPath = Application.dataPath + "/SayKit/Internal/Plugins/Settings/gradle-wrapper.properties";
        File.Copy(gradleWrapperPath, projectPath + "/gradle/wrapper/gradle-wrapper.properties", true);
    }

    private void UpdateSayKitGradleFile(string path)
    {
        var projectPath = string.Copy(path);
        projectPath = projectPath.Replace("launcher", "");

        var gradleWrapperPath = Application.dataPath + "/SayKit/Internal/Plugins/Settings/saykitBuildTemplate.gradle";
        File.Copy(gradleWrapperPath, projectPath + "unityLibrary/saykit/build.gradle", true);
    }

}

#endif