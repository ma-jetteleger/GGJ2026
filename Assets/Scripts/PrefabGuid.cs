using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class PrefabGuid : MonoBehaviour
{
    [SerializeField] private string guid;

    public string Guid => guid;

    private void Awake()
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString("N");
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Regenerate GUID")]
    private void RegenerateGuid()
    {
        guid = System.Guid.NewGuid().ToString("N");
        EditorUtility.SetDirty(this);
    }

    public void RegenerateGuidEditor()
    {
        RegenerateGuid();
    }
#endif
}
