using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PrefabGuidTools
{
    private const string MenuPath = "Assets/Add Prefab GUID Component";

    [MenuItem(MenuPath, true)]
    private static bool AddPrefabGuidValidate()
    {
        return GetSelectedPrefabAssets().Count > 0;
    }

    [MenuItem(MenuPath)]
    private static void AddPrefabGuid()
    {
        var prefabs = GetSelectedPrefabAssets();
        if (prefabs.Count == 0)
        {
            return;
        }

        foreach (var prefab in prefabs)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                var guidComponent = root.GetComponent<PrefabGuid>();
                if (guidComponent == null)
                {
                    guidComponent = root.AddComponent<PrefabGuid>();
                }

                guidComponent.RegenerateGuidEditor();
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        AssetDatabase.SaveAssets();
    }

    private static List<GameObject> GetSelectedPrefabAssets()
    {
        var selection = Selection.GetFiltered<GameObject>(SelectionMode.Assets);
        var prefabs = new List<GameObject>(selection.Length);

        foreach (var asset in selection)
        {
            if (PrefabUtility.GetPrefabAssetType(asset) == PrefabAssetType.NotAPrefab)
            {
                continue;
            }

            prefabs.Add(asset);
        }

        return prefabs;
    }
}
