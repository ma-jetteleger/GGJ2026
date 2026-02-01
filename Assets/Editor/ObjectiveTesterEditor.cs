using UnityEngine;
using UnityEditor; // Required for custom inspector code

[CustomEditor(typeof(ObjectiveTester))]
public class ObjectiveTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (so we still see the prefab slot)
        DrawDefaultInspector();

        ObjectiveTester tester = (ObjectiveTester)target;

        GUILayout.Space(10); // Add some breathing room

        // Create the button
        if (GUILayout.Button("Show on Show3DObjective Right Now", GUILayout.Height(30)))
        {
            // This allows it to work while the game is running (Play Mode)
            if (Application.isPlaying)
            {
                tester.TestDisplay();
            }
            else
            {
                Debug.LogWarning("You must be in Play Mode to test the UI display!");
            }
        }
    }
}