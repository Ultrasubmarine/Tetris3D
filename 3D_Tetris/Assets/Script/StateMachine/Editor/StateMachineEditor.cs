using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UI;

[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor {
    StateMachine component;

    SerializedProperty table;
    SerializedProperty text1;
    int _needCount;

    private void OnEnable() {
        table = serializedObject.FindProperty("_StateTable");
        text1 = serializedObject.FindProperty("_UIText");
        _needCount = Enum.GetValues(typeof(EMachineState)).Length ;

        component = (StateMachine)target;
    }

    public override void OnInspectorGUI() {

        serializedObject.Update();

        if (table.arraySize != _needCount * _needCount)
            ChangeTable();

        EditorGUILayout.LabelField("Таблица переходов:");

        for (int row = 0; row < _needCount; row++) {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(((EMachineState)row).ToString());
            for (int column = 0; column < _needCount; column++) {

               var yach = table.GetArrayElementAtIndex(row * _needCount + column);
                yach.boolValue = GUILayout.Toggle(yach.boolValue,"");
            }
            EditorGUILayout.EndHorizontal();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void ChangeTable() {
        table.arraySize = _needCount* _needCount;      
    }
}
