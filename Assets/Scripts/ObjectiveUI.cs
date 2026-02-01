using UnityEngine;
using TMPro;

[RequireComponent(typeof(Camera))]
public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("3D Setup")]
    [Tooltip("Layer for the UI Camera (Must match exactly).")]
    [SerializeField] private string targetLayerName = "UI_3D_Render";

    [Header("Model Settings")]
    [Range(1f, 2f)]
    [Tooltip("1.0 = Touch edges. 1.2 = Nice padding.")]
    [SerializeField] private float framingMargin = 1.1f; 
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Vector3 defaultRotation = new Vector3(0, 150, 0);
    [SerializeField] private PrefabLibrary prefabLibrary = null;

    // Internal
    private GameObject _currentModel;
    private int _cachedLayerId;
    private Camera _cam;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _cam = GetComponent<Camera>();
        _cachedLayerId = LayerMask.NameToLayer(targetLayerName);
    }

    private void Update()
    {
        if (_currentModel != null)
        {
            // Rotate the model in place
            _currentModel.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    // --- API ---

    public void UpdateObjective(string newObjective)
    {
        if (objectiveText != null) objectiveText.text = newObjective;
    }

    public void Show3DObjective(GameObject prefabToDisplay)
    {
        if (_currentModel != null) Destroy(_currentModel);

        if (prefabToDisplay != null)
        {
            // 1. Instantiate as child of the Camera (Transform)
            _currentModel = Instantiate(prefabToDisplay, transform);
            
            // 2. Reset standard transform values
            _currentModel.transform.localRotation = Quaternion.Euler(defaultRotation);
            _currentModel.transform.localScale = Vector3.one;
            _currentModel.transform.localPosition = Vector3.zero; // Start at zero for calculations

            // 3. Force physics engine to update so bounds are correct immediately
            Physics.SyncTransforms();

            // 4. Run the fitting logic using Colliders
            FitByCollider(_currentModel);

            // 5. Set Layer
            SetLayerRecursively(_currentModel, _cachedLayerId);

            objectiveText.text = prefabLibrary.GetPrettyName(prefabToDisplay);
        }
    }

    // --- FIT LOGIC ---

    private void FitByCollider(GameObject model)
    {
        // Try to find a collider on the parent or children
        Collider col = model.GetComponentInChildren<Collider>();
        
        // If your prefabs have multiple colliders (compound), you might need GetComponentsInChildren
        // For now, let's assume the main bulk is defined by the first found collider.
        if (col == null) 
        {
            Debug.LogWarning($"[ObjectiveUI] Prefab '{model.name}' has no Collider! Fitting failed.");
            model.transform.localPosition = new Vector3(0, 0, 5); // Fallback
            return;
        }

        // A. Get World Bounds of the collider
        Bounds bounds = col.bounds;

        // B. Calculate the "Radius" of the object
        // We use the magnitude of the extents (corner to center) to ensure 
        // the object stays in frame even when it spins.
        float objectRadius = bounds.extents.magnitude;

        // C. Calculate Distance
        // Formula: Distance = Radius / sin(FOV/2) 
        // (Note: Using Sin for sphere fitting is often safer than Tan for box fitting)
        float halfFovRad = _cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float distance = objectRadius / Mathf.Sin(halfFovRad);
        
        distance *= framingMargin;

        // D. Center Correction (The "Pivot Problem")
        // We want the CENTER of the collider to end up at (0, 0, distance).
        // Currently, the model Pivot is at (0,0,0).
        // We calculate the offset from the Pivot to the Bounds Center.
        Vector3 pivotToCenter = bounds.center - model.transform.position;

        // In Camera local space, we want the center to be at Z = distance
        Vector3 targetCenter = new Vector3(0, 0, distance);

        // Apply position: Target - Offset
        // We use InverseTransformVector because we are working in Local Space relative to camera
        Vector3 adjustment = transform.InverseTransformVector(pivotToCenter);
        
        model.transform.localPosition = targetCenter - adjustment;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach(Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}