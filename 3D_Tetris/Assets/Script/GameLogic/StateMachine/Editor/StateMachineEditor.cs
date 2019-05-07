using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor {

    SerializedProperty table;
    int _needCount;

    private void OnEnable() {
        table = serializedObject.FindProperty("StateTable");
        _needCount = Enum.GetValues(typeof(GameState2)).Length ;
    }

    public override void OnInspectorGUI() {

        serializedObject.Update();

        if (table.arraySize != _needCount * _needCount)
            ChengeTable();


        EditorGUILayout.LabelField("Таблица переходов:");

        // EditorGUILayout.BeginVertical();
        for (int row = 0; row < _needCount; row++) {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(((GameState2)row).ToString());
            for (int column = 0; column < _needCount; column++) {

               var yach = table.GetArrayElementAtIndex(row * _needCount + column);
                yach.boolValue = GUILayout.Toggle(yach.boolValue,"");

            }
            EditorGUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void ChengeTable() {
        Debug.Log("ПЫТАЕМСЯ ИЗМЕНИТЬ РАЗМЕР");

        table.arraySize = _needCount* _needCount;// = _needCount;    }
        Debug.Log(" " +table.arraySize.ToString()  +  " need " + _needCount.ToString());
        
    }
}
