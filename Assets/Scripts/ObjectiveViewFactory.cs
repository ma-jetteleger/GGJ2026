using UnityEngine;

public class ObjectiveViewFactory : MonoBehaviour
{
    public static ObjectiveViewFactory Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private ObjectiveView objectiveCameraPrefab;
    [SerializeField] private Vector2Int defaultTextureSize = new Vector2Int(512, 512);
    
    [Header("Spawn Settings")]
    [SerializeField] private Vector3 spawnOrigin = new Vector3(0, -10000, 0);
    [SerializeField] private Vector3 spawnOffsetStep = new Vector3(100, 0, 0);

    private int _spawnCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Creates a new self-contained 3D view for the given prefab.
    /// Returns the Render Result which contains the Texture to display and the View controller.
    /// </summary>
    public ObjectiveViewResult CreateView(GameObject prefabToDisplay)
    {
        if (objectiveCameraPrefab == null)
        {
            Debug.LogError("[ObjectiveViewFactory] No Camera Prefab assigned!");
            return null;
        }

        // 1. Determine spawn position
        Vector3 spawnPos = spawnOrigin + (spawnOffsetStep * _spawnCount);
        _spawnCount++;

        // 2. Instantiate the Camera Prefab
        ObjectiveView newView = Instantiate(objectiveCameraPrefab, spawnPos, Quaternion.identity);
        newView.gameObject.name = $"ObjectiveView_{prefabToDisplay.name}_{_spawnCount}";

        // 3. Create Render Texture
        RenderTexture rt = new RenderTexture(defaultTextureSize.x, defaultTextureSize.y, 16);
        rt.name = $"RT_{prefabToDisplay.name}";
        
        // 4. Assign RT to Camera
        Camera cam = newView.GetComponent<Camera>();
        cam.targetTexture = rt;

        // 5. Initialize the View
        newView.Show3DObjective(prefabToDisplay);

        return new ObjectiveViewResult(rt, newView);
    }
}

public class ObjectiveViewResult
{
    public RenderTexture Texture { get; private set; }
    public ObjectiveView View { get; private set; }

    public ObjectiveViewResult(RenderTexture texture, ObjectiveView view)
    {
        Texture = texture;
        View = view;
    }

    public void Destroy()
    {
        if (View != null) Object.Destroy(View.gameObject);
        if (Texture != null) Object.Destroy(Texture);
    }
}
