using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(ConsumableSpawner))]
public class ConsumableInspector : Editor
{
    ConsumableSpawner spawner;
    SerializedProperty spawnAreaSize;
    SerializedProperty spawnAreaRotation;
    
    void OnEnable()
    {
        spawner = (ConsumableSpawner)target;
        spawnAreaSize = serializedObject.FindProperty("spawnAreaSize");
        spawnAreaRotation = serializedObject.FindProperty("spawnAreaRotation");
    }

    void OnSceneGUI()
    {        
        serializedObject.Update();

        var transform = spawner.transform;
        var halfAreaSize = spawnAreaSize.vector3Value * 0.5f;

        // handle size change
        EditorGUI.BeginChangeCheck();

        var handleSize = HandleUtility.GetHandleSize(transform.position) * 0.1f;
        var handleSnap = 0.5f;

        var posX = transform.position + new Vector3(halfAreaSize.x, 0f, 0f);
        Handles.color = Handles.xAxisColor;
        var newX = Handles.Slider(posX, Vector3.right, handleSize, Handles.ConeHandleCap, handleSnap).x - transform.position.x;
       
        var posY = transform.position + new Vector3(0f, halfAreaSize.y, 0f);
        Handles.color = Handles.yAxisColor;
        var newY = Handles.Slider(posY, Vector3.up, handleSize, Handles.ConeHandleCap, handleSnap).y - transform.position.y;

        var posZ = transform.position + new Vector3(0f, 0f, halfAreaSize.z);
        Handles.color = Handles.zAxisColor;
        var newZ = Handles.Slider(posZ, Vector3.forward, handleSize, Handles.ConeHandleCap, handleSnap).z - transform.position.z;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spawner, "Modified Spawn Bounds");
            spawnAreaSize.vector3Value = new Vector3(newX, newY, newZ) * 2f;
        }     

        serializedObject.ApplyModifiedProperties();
    }
}
