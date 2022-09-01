#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class SayKitWindow : EditorWindow
{
    public static void Init()
    {
        const int width = 400;
        const int height = 150;

        SayKitWindow window = (SayKitWindow)EditorWindow.GetWindow(typeof(SayKitWindow));
        window.position = new Rect(0, 0, width, height);
        window.Show();
    }

    void OnGUI()
    {
        
        GUILayout.Label("SayKit Settings", EditorStyles.boldLabel);

        GUILayout.Space(3);
        GUILayout.BeginHorizontal();
        SayKitSettings.Instance.SecretKey_iOS = EditorGUILayout.TextField("Secret iOS:", SayKitSettings.Instance.SecretKey_iOS);
        GUILayout.EndHorizontal();
        GUILayout.Space(1);
        GUILayout.BeginHorizontal();
        SayKitSettings.Instance.SecretKey_Android = EditorGUILayout.TextField("Secret Android:", SayKitSettings.Instance.SecretKey_Android);
        GUILayout.EndHorizontal();

        GUILayout.Space(3);


        if (GUI.changed)
        {
            SayKitSettings.Instance.UpdateAssets();
        }
    }

    void OnDestroy()
    {
        var stringsPath = "Assets/SayKit/SayKitKey.cs";
        var lines = new List<string>
                    {
                        "public class SayKitKey",
                        "{",
                        "      public static string SECRET_IOS = \""  + SayKitSettings.Instance.SecretKey_iOS + "\";",
                        "      public static string SECRET_ANDROID = \""  + SayKitSettings.Instance.SecretKey_Android + "\";",
                        "}"
                    };

        File.WriteAllLines(stringsPath, lines);
        UnityEditor.AssetDatabase.Refresh();
    }
}



public class SayKitSettings : ScriptableObject
{
    [SerializeField] private string Secret_iOS;
    public string SecretKey_iOS
    {
        get { return Instance.Secret_iOS; }
        set { Instance.Secret_iOS = value; }
    }

    [SerializeField] private string Secret_Android;
    public string SecretKey_Android
    {
        get { return Instance.Secret_Android; }
        set { Instance.Secret_Android = value; }
    }


    private static SayKitSettings _instance;
    public static SayKitSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateInstance<SayKitSettings>();
                _instance.Secret_iOS = SayKitKey.SECRET_IOS;
                _instance.Secret_Android = SayKitKey.SECRET_ANDROID;
            }

            return _instance;
        }
    }


    public void UpdateAssets()
    {
        EditorUtility.SetDirty(_instance);
    }
}

#endif

