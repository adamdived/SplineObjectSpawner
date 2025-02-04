using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineObjectSpawner))]
public class SplineObjectSpawnerEditor : Editor
{
    private bool showSplineSettings = true;
    private bool showObjectGroups = true;
    private bool showPlacementSettings = true;
    private bool showRotationSettings = true;

    public override void OnInspectorGUI()
    {
        SplineObjectSpawner spawner = (SplineObjectSpawner)target;
        
        // Custom styling
        GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
        headerStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.Space(10);

        // Spline Settings
        showSplineSettings = EditorGUILayout.Foldout(showSplineSettings, "Spline Settings", true, headerStyle);
        if (showSplineSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("splineContainer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetObjectDensity"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainLayer"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(10);

        // Object Groups
        showObjectGroups = EditorGUILayout.Foldout(showObjectGroups, "Object Groups", true, headerStyle);
        if (showObjectGroups)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Group 1", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group1Prefabs"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group1Radius"));
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Group 2", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group2Prefabs"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group2Radius"));
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Group 3", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group3Prefabs"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("group3Radius"));
            
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(10);

        // Placement Settings
        showPlacementSettings = EditorGUILayout.Foldout(showPlacementSettings, "Placement Settings", true, headerStyle);
        if (showPlacementSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomScaleRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minDistanceBetweenObjects"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomOffsetRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fixedYOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSlopeAngle"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(10);

        // Rotation Settings
        showRotationSettings = EditorGUILayout.Foldout(showRotationSettings, "Rotation Settings", true, headerStyle);
        if (showRotationSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomRotationRangeX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomRotationRangeY"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomRotationRangeZ"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(15);

        // Update button with custom styling
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Update Spawn", GUILayout.Height(30)))
        {
            spawner.SpawnObjects();
        }
        GUI.backgroundColor = Color.white;

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
