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
        table = serializedObject.FindProperty("StateTable");
        text1 = serializedObject.FindProperty("UIText");
        _needCount = Enum.GetValues(typeof(EMachineState)).Length ;

        component = (StateMachine)target;
    }

    public override void OnInspectorGUI() {

        serializedObject.Update();

        if (table.arraySize != _needCount * _needCount)
            ChengeTable();

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

        component._UiText = (Text)EditorGUILayout.ObjectField("State Text", component._UiText, typeof(Text), true);
        serializedObject.ApplyModifiedProperties();
    }

    private void ChengeTable() {
        table.arraySize = _needCount* _needCount;      
    }
}
