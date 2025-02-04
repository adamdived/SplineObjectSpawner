using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineObjectSpawner))]
public class SplineObjectSpawnerEditor : Editor
{
    private bool _showSplineSettings = true;
    private bool _showObjectGroups = true;
    private bool _showPlacementSettings = true;
    private bool _showRotationSettings = true;

    public override void OnInspectorGUI()
    {
        SplineObjectSpawner spawner = (SplineObjectSpawner)target;
        
        // Custom styling
        GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.margin = new RectOffset(15, 0, 0, 0);  // Add left margin to foldout

        // Box style
        GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
        boxStyle.padding = new RectOffset(20, 10, 5, 10);  // Increased left padding
        boxStyle.margin = new RectOffset(0, 0, 10, 10);

        EditorGUILayout.Space(10);

        // Spline Settings
        using (new EditorGUILayout.VerticalScope(boxStyle))
        {
            EditorGUILayout.Space(5);  // Add space before foldout
            _showSplineSettings = EditorGUILayout.Foldout(_showSplineSettings, "Spline Settings", true, headerStyle);
            if (_showSplineSettings)
            {
                EditorGUILayout.Space(5);  // Add space after foldout
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("splineContainer"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("targetObjectDensity"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainLayer"));
                EditorGUI.indentLevel--;
            }
        }

        // Object Groups
        using (new EditorGUILayout.VerticalScope(boxStyle))
        {
            EditorGUILayout.Space(5);
            _showObjectGroups = EditorGUILayout.Foldout(_showObjectGroups, "Object Groups", true, headerStyle);
            if (_showObjectGroups)
            {
                EditorGUILayout.Space(5);
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("Group 1", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("group1Prefabs"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("group1Radius"));
                }
                
                EditorGUILayout.Space(5);
                
                EditorGUILayout.LabelField("Group 2", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("group2Prefabs"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("group2Radius"));
                }
                
                EditorGUILayout.Space(5);
                
                EditorGUILayout.LabelField("Group 3", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("group3Prefabs"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("group3Radius"));
                }
                
                EditorGUI.indentLevel--;
            }
        }

        // Placement Settings
        using (new EditorGUILayout.VerticalScope(boxStyle))
        {
            EditorGUILayout.Space(5);
            _showPlacementSettings = EditorGUILayout.Foldout(_showPlacementSettings, "Placement Settings", true, headerStyle);
            if (_showPlacementSettings)
            {
                EditorGUILayout.Space(5);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("randomScaleRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minDistanceBetweenObjects"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("randomOffsetRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fixedYOffset"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSlopeAngle"));
                EditorGUI.indentLevel--;
            }
        }

        // Rotation Settings
        using (new EditorGUILayout.VerticalScope(boxStyle))
        {
            EditorGUILayout.Space(5);
            _showRotationSettings = EditorGUILayout.Foldout(_showRotationSettings, "Rotation Settings", true, headerStyle);
            if (_showRotationSettings)
            {
                EditorGUILayout.Space(5);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("randomRotationRangeX"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("randomRotationRangeY"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("randomRotationRangeZ"));
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.Space(15);

        // Update button with custom styling
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.padding = new RectOffset(10, 10, 5, 5);
        buttonStyle.fontStyle = FontStyle.Bold;

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);
            if (GUILayout.Button("Update Spawn", buttonStyle, GUILayout.Height(30)))
            {
                spawner.SpawnObjects();
            }
            GUI.backgroundColor = Color.white;
        }

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
