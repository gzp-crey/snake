using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConsumableSpawner))]
public class ConsumableSpawnerInspector : Editor
{
    ConsumableSpawner spawner;
    SerializedProperty spawnAreaSize;

    void OnEnable()
    {
        spawner = (ConsumableSpawner)target;
        spawnAreaSize = serializedObject.FindProperty("spawnAreaSize");
    }

    void OnSceneGUI()
    {
        serializedObject.Update();

        var transform = spawner.transform;
        var halfAreaSize = spawnAreaSize.vector2Value * 0.5f;

        // handle size change
        EditorGUI.BeginChangeCheck();

        var handleSize = HandleUtility.GetHandleSize(transform.position) * 0.1f;
        var handleSnap = 0.5f;

        var posX = transform.position + new Vector3(halfAreaSize.x, 0f, 0f);
        Handles.color = Handles.xAxisColor;
        var newX = Handles.Slider(posX, Vector3.right, handleSize, Handles.ConeHandleCap, handleSnap).x - transform.position.x;

        var posZ = transform.position + new Vector3(0f, 0f, halfAreaSize.y);
        Handles.color = Handles.zAxisColor;
        var newY = Handles.Slider(posZ, Vector3.forward, handleSize, Handles.ConeHandleCap, handleSnap).z - transform.position.z;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spawner, "Modified Spawn Bounds");
            spawnAreaSize.vector2Value = new Vector2(newX, newY) * 2f;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
