#if UNITY_EDITOR
using UnityEditor;

public class SayKitEditor
{
    public const string DIALOG_TITLE = "SayKit Message";

    [MenuItem("SayKit/Settings Manager")]
    private static void IntegrationManager()
    {
        SayKitWindow.Init();
    }

    [MenuItem("SayKit/Check Release build")]
    private static void EmbedConfig()
    {
        var pb = new SayKitPreBuild();
        SayKitPreBuild.buildPlace = "check_release_build";

        pb.OnPreprocessBuild(null, true);
        if (!pb.dirtyFlag)
        {
            EditorUtility.DisplayDialog(DIALOG_TITLE, "Finished without errors!\n\nᕦ( ͡° ͜ʖ ͡°)ᕤ", "Awesome!");
        }
    }
}

#endif