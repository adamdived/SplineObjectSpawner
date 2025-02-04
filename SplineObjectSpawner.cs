using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

// This script spawns objects along a spline and updates their positions when the spline changes.
// by Marco Capelli

[ExecuteInEditMode] // Ensures the script runs in the editor.
public class SplineObjectSpawner : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer; // Reference to the spline container.
    public GameObject[] group1Prefabs; // Prefabs for the first group of objects.
    public float group1Radius = 2.0f; // Radius offset for the first group.
    public GameObject[] group2Prefabs; // Prefabs for the second group of objects.
    public float group2Radius = 1.5f; // Radius offset for the second group.
    public GameObject[] group3Prefabs; // Prefabs for the third group of objects.
    public float group3Radius = 1.0f; // Radius offset for the third group.
    public float targetObjectDensity = 8.0f; // Density of objects along the spline.
    public Vector2 randomScaleRange = new Vector2(1.0f, 2.0f); // Random scale range for spawned objects.
    public float minDistanceBetweenObjects = 4.0f; // Minimum distance between spawned objects.
    [SerializeField] private LayerMask terrainLayer = 1; // Layer mask for terrain detection.
    public Vector2 randomOffsetRange = new Vector2(-3.0f, 3.0f); // Random offset range for object placement.
    public Vector2 randomRotationRangeY = new Vector2(0.0f, 360f); // Random Y rotation range.
    public Vector2 randomRotationRangeX = new Vector2(-5f, 5f); // Random X rotation range.
    public Vector2 randomRotationRangeZ = new Vector2(-5f, 5f); // Random Z rotation range.
    public float fixedYOffset = -0.3f; // Fixed Y offset for object placement.
    public float maxSlopeAngle = 30.0f; // Maximum slope angle for object placement.

    private GameObject[] _group1Instances; // Instances of the first group of objects.
    private GameObject[] _group2Instances; // Instances of the second group of objects.
    private GameObject[] _group3Instances; // Instances of the third group of objects.

    private Vector3 _lastSplineContainerPosition; // Last recorded position of the spline container.

    private void Awake()
    {
        // Automatically assign the SplineContainer component if it's not set.
        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>();
        }
    }

    private void OnEnable()
    {
        if (splineContainer == null) return;

        // Subscribe to spline change and editor update events.
        Spline.Changed += OnSplineChanged;
        EditorApplication.update += OnEditorUpdate;

        // Spawn objects and record the spline container's position.
        SpawnObjects();
        _lastSplineContainerPosition = splineContainer.transform.position;
    }

    private void OnDisable()
    {
        // Unsubscribe from events when the script is disabled.
        Spline.Changed -= OnSplineChanged;
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification)
    {
        // Update object positions when the spline changes.
        UpdateObjectPositions();
    }

    private void OnEditorUpdate()
    {
        // Update object positions if the spline container's position changes.
        if (splineContainer != null && splineContainer.transform.position != _lastSplineContainerPosition)
        {
            _lastSplineContainerPosition = splineContainer.transform.position;
            UpdateObjectPositions();
        }
    }

    public void SpawnObjects()
    {
        // Clear existing objects and spawn new ones.
        ClearObjects();

        if (group1Prefabs != null && group1Prefabs.Length > 0)
            _group1Instances = SpawnGroup(group1Prefabs, group1Radius, true);

        if (group2Prefabs != null && group2Prefabs.Length > 0)
            _group2Instances = SpawnGroup(group2Prefabs, group2Radius, false);

        if (group3Prefabs != null && group3Prefabs.Length > 0)
            _group3Instances = SpawnGroup(group3Prefabs, group3Radius, true);
    }

    private GameObject[] SpawnGroup(GameObject[] prefabs, float radius, bool usePositiveNormal)
    {
        // Spawn objects along the spline for a specific group.
        if (splineContainer == null || prefabs == null || prefabs.Length == 0)
            return null;

        var spline = splineContainer.Spline;
        float splineLength = spline.GetLength();
        int numObjects = Mathf.FloorToInt(splineLength * targetObjectDensity);
        if (numObjects < 1) numObjects = 1;

        GameObject[] instances = new GameObject[numObjects];
        int spawnedCount = 0;

        for (int i = 0; i < numObjects; i++)
        {
            float t = (float)i / numObjects;
            Vector3 localPosition = spline.EvaluatePosition(t);
            Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
            Vector3 tangent = spline.EvaluateTangent(t);
            Vector3 forward = Vector3.ProjectOnPlane(tangent, Vector3.up).normalized; // Ignore spline knot rotations
            Vector3 normal = Vector3.Cross(forward, Vector3.up).normalized;

            if (!usePositiveNormal)
                normal = -normal;

            Vector3 spawnPosition = worldPosition + normal * radius;
            spawnPosition += new Vector3(Random.Range(randomOffsetRange.x, randomOffsetRange.y), 0, Random.Range(randomOffsetRange.x, randomOffsetRange.y));
            spawnPosition = GetTerrainHeight(spawnPosition);

            if (GetTerrainSlope(spawnPosition) > maxSlopeAngle)
                continue;

            spawnPosition.y += fixedYOffset;

            if (!IsOverlapping(spawnPosition, instances, spawnedCount))
            {
                GameObject prefab = prefabs[spawnedCount % prefabs.Length];
                instances[spawnedCount] = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);

                // Set rotation to face the forward direction (ignoring spline knot rotations)
                Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
                float randomYRotation = Random.Range(randomRotationRangeY.x, randomRotationRangeY.y);
                float randomXRotation = Random.Range(randomRotationRangeX.x, randomRotationRangeX.y);
                float randomZRotation = Random.Range(randomRotationRangeZ.x, randomRotationRangeZ.y);
                Quaternion randomRotation = Quaternion.Euler(randomXRotation, randomYRotation, randomZRotation);
                instances[spawnedCount].transform.rotation = rotation * randomRotation;

                float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                instances[spawnedCount].transform.localScale = Vector3.one * randomScale;

                spawnedCount++;
            }
        }

        System.Array.Resize(ref instances, spawnedCount);
        return instances;
    }

    private Vector3 GetTerrainHeight(Vector3 position)
    {
        // Adjust the Y position of the object to match the terrain height.
        Ray ray = new Ray(position + Vector3.up * 1000f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
        {
            position.y = hit.point.y;
        }
        return position;
    }

    private float GetTerrainSlope(Vector3 position)
    {
        // Calculate the slope angle at the given position.
        Ray ray = new Ray(position + Vector3.up * 1000f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
        {
            Vector3 normal = hit.normal;
            return Vector3.Angle(normal, Vector3.up);
        }
        return 0;
    }

    private bool IsOverlapping(Vector3 position, GameObject[] instances, int count)
    {
        // Check if the new object position overlaps with existing objects.
        for (int i = 0; i < count; i++)
        {
            if (Vector3.Distance(position, instances[i].transform.position) < minDistanceBetweenObjects)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateObjectPositions()
    {
        // Update positions for all groups of objects.
        if (_group1Instances != null && _group1Instances.Length > 0)
            UpdateGroupPositions(_group1Instances, group1Radius, true);

        if (_group2Instances != null && _group2Instances.Length > 0)
            UpdateGroupPositions(_group2Instances, group2Radius, false);

        if (_group3Instances != null && _group3Instances.Length > 0)
            UpdateGroupPositions(_group3Instances, group3Radius, true);
    }

    private void UpdateGroupPositions(GameObject[] instances, float radius, bool usePositiveNormal)
    {
        // Update positions for a specific group of objects.
        if (splineContainer == null || instances == null)
            return;

        var spline = splineContainer.Spline;
        float splineLength = spline.GetLength();
        int numObjects = instances.Length;

        for (int i = 0; i < numObjects; i++)
        {
            float t = (float)i / numObjects;
            Vector3 localPosition = spline.EvaluatePosition(t);
            Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
            Vector3 tangent = spline.EvaluateTangent(t);
            Vector3 forward = Vector3.ProjectOnPlane(tangent, Vector3.up).normalized; // Ignore spline knot rotations
            Vector3 normal = Vector3.Cross(forward, Vector3.up).normalized;

            if (!usePositiveNormal)
                normal = -normal;

            Vector3 spawnPosition = worldPosition + normal * radius;
            spawnPosition += new Vector3(Random.Range(randomOffsetRange.x, randomOffsetRange.y), 0, Random.Range(randomOffsetRange.x, randomOffsetRange.y));
            spawnPosition = GetTerrainHeight(spawnPosition);
            spawnPosition.y += fixedYOffset;

            instances[i].transform.position = spawnPosition;

            // Set rotation to face the forward direction (ignoring spline knot rotations)
            Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
            instances[i].transform.rotation = rotation;
        }
    }

    private void ClearObjects()
    {
        // Clear all spawned objects.
        DestroyImmediateChildren();
        _group1Instances = null;
        _group2Instances = null;
        _group3Instances = null;
    }

    private void DestroyImmediateChildren()
    {
        // Destroy all child objects of this GameObject.
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
