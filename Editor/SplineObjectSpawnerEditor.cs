using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineObjectSpawner))]
public class SplineObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get a reference to the SplineObjectSpawner component
        SplineObjectSpawner spawner = (SplineObjectSpawner)target;

        // Add a button to manually update the spawn
        if (GUILayout.Button("Update Spawn"))
        {
            spawner.SpawnObjects();
        }
    }
}
