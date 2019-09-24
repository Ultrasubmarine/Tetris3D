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
    bool showPosition;
    EMachineState _key;
    Dictionary<StateMachine, List<EMachineState>> States = new Dictionary<StateMachine, List<EMachineState>>();
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

            EditorGUILayout.LabelField(((EMachineState)row).ToString(), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            OneStateInGUI(  (EMachineState)row);
//            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.LabelField(((EMachineState)row).ToString());
//            for (int column = 0; column < _needCount; column++) {
//
//               var yach = table.GetArrayElementAtIndex(row * _needCount + column);
//                yach.boolValue = GUILayout.Toggle(yach.boolValue,"");
//            }
//            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            if(GUILayout.Button("Add state connect"))
            {
                Debug.Log(" empty. add item connect");
            }
        }
        
       
        serializedObject.ApplyModifiedProperties();
    }
    
    /*
     * for (int column = 0; column < _needCount; column++) {

               var yach = table.GetArrayElementAtIndex(row * _needCount + column);
                yach.boolValue = GUILayout.Toggle(yach.boolValue,"");
            }
     */

    private void ChangeTable() {
        table.arraySize = _needCount* _needCount;      
    }

    public void OneStateInGUI( EMachineState state) {
        
        EditorGUILayout.BeginHorizontal();
        _key = (EMachineState) EditorGUILayout.EnumPopup("state", _key);
        
        if(GUILayout.Button("(-) delete"))
        {
            Debug.Log(" empty. delete item");
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DeleteStateConnection() {
        
    }
}
