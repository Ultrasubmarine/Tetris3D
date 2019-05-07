using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(AudioItem))]
[CanEditMultipleObjects]
public class EditorAudioItem : Editor {

    SerializedProperty AudioObjList;
    SerializedProperty CurrentAudio;

    // audio names
    List<string> options = new List<string>() { "hhh", "ccc", "gg"};// { "Cube", "Sphere", "Plane" };
    public int index = 0;

    // Use this for initialization
    void OnEnable()
    {
        AudioObjList = serializedObject.FindProperty("_AudioList");
        CurrentAudio = serializedObject.FindProperty("_CurrentAudio");
       
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("поле для выбора аудио:");
        InicializationAudioList();
        index = EditorGUILayout.Popup(index, options.ToArray());

        // add ScreenObj item
        for (int i=0; i< AudioObjList.arraySize; i++)       
        {
            //EditorGUILayout.Popup();
            //if (AnimationsObjList.arraySize - 1 < i)
            //    AnimationsObjList.arraySize++;
            //var prop = AnimationsObjList.GetArrayElementAtIndex(i);

            //// Field
            //EditorGUILayout.PropertyField(prop, new GUIContent(((Win)i++).ToString()));
        }

        serializedObject.ApplyModifiedProperties();
    }

    void InicializationAudioList()
    {
        options.Clear();

        for (int i=0; i< AudioObjList.arraySize; i++)
        {
            // sp.objectReferenceValue
            // AudioClip itemClip =(AudioClip) ;
               string pathItem = serializedObject.FindProperty("_AudioList[0].Audio.name").ToString();// CurrentAudio.propertyPath;// serializedObject.FindProperty("_AudioList(1).name").ToString();
            //string pathItem = serializedObject.FindProperty("_AudioList").GetArrayElementAtIndex(i).FindPropertyRelative("name").ToString();
            //  AnimationClip j = (AnimationClip)((object)(AudioObjList.GetArrayElementAtIndex(i)));
            options.Add(pathItem);//.serializedObject.FindProperty("name").ToString());
        }
    }

    
}
