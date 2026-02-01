using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabLibrary", menuName = "ScriptableObjects/PrefabLibrary")]
public class PrefabLibrary : ScriptableObject, ISerializationCallbackReceiver
{
    [System.Serializable]
    public struct Entry
    {
        public string PrettyName;
        public GameObject Prefab; // Must be GameObject to accept standard prefabs
    }

    // Change this to public to see if it fixes the inspector lock
    public Entry[] Entries; 
    
    private Dictionary<GameObject, string> _reverseCache = new Dictionary<GameObject, string>();

    public string GetPrettyName(GameObject prefab)
    {
        if (prefab == null) return "None";
        return _reverseCache.TryGetValue(prefab, out string name) ? name : "Unknown";
    }

    public void OnAfterDeserialize()
    {
        _reverseCache.Clear();
        if (Entries == null) return;

        foreach (var entry in Entries)
        {
            if (entry.Prefab != null && !_reverseCache.ContainsKey(entry.Prefab))
            {
                _reverseCache.Add(entry.Prefab, entry.PrettyName);
            }
        }
    }

    public void OnBeforeSerialize() { }
}