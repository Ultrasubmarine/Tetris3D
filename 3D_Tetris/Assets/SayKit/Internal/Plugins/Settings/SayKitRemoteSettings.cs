using System;
using System.Collections.Generic;
using System.IO;
using SayKitInternal;
using UnityEngine;

public class SayKitRemoteSettings
{
    public static SayKitRemoteSettings SharedInstance = new SayKitRemoteSettings();

    [Serializable]
    public class SayKitRemoteData
    {
        public string[] Errors;
        public string[] Messages;

        public string FacebookAppId;
        public string FacebookAppName;
        public string AdjustToken;
    }

    public static string buildPlace = "default";

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

    public SayKitRemoteSettings()
    {
        string url = GetRemoteURL();

        SayKitWebRequest sayKitWebRequest = new SayKitWebRequest(url);
        sayKitWebRequest.SendAndWait(10);

        String data = "";

        try
        {
            if (sayKitWebRequest.IsDone && string.IsNullOrEmpty(sayKitWebRequest.ErrorMessage))
            {
                if (sayKitWebRequest.Text.Length > 0 && sayKitWebRequest.Text[0] == '{')
                {
                    data = sayKitWebRequest.Text;
                    Debug.LogWarning("Config data was downloaded!");
                }
            }
        }
        catch { }



        var config = JsonUtility.FromJson<SayKitRemoteData>(data);

        if (config.Errors?.Length == 0)
        {
            facebook_app_id = config.FacebookAppId;
            facebook_app_name = config.FacebookAppName;
        }
        else
        {
            Debug.LogError("saykit_settings file is missed! Please, check a Assets/Resources/saykit_settings.json");
        }


        if (String.IsNullOrEmpty(facebook_app_name)
                || String.IsNullOrEmpty(facebook_app_id))
        {
            Debug.LogError("saykit_settings file doesn't contain configuration data! Please, check a Assets/Resources/saykit_settings.json");
        }
        else
        {
            SayKitDebug.Log("saykit_settings is successfully initialized. "
                + "\n facebook_app_id: " + facebook_app_id
                + "\n facebook_app_name: " + facebook_app_name);
        }

    }

    public string facebook_app_id = "";
    public string facebook_app_name = "";

}
