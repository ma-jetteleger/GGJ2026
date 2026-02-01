using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabGuid))]
public class PrefabGuidEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var guidProperty = serializedObject.FindProperty("guid");
        serializedObject.Update();

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(guidProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
