using UnityEngine;

public class ObjectiveTester : MonoBehaviour
{
    [Header("Test Settings")]
    public GameObject prefabToTest;
    public string testObjectiveText = "Test Objective Loaded!";

    // This method is called by the custom button in the Inspector
    public void TestDisplay()
    {
        if (prefabToTest == null)
        {
            Debug.LogWarning("Assign a prefab to test first!");
            return;
        }

        if (ObjectiveUI.Instance == null)
        {
            Debug.LogError("ObjectiveUI Instance not found! Make sure the 'ObjectiveUI' script is attached to a GameObject in your scene.");
            return;
        }

        // 1. Update Text
        // ObjectiveUI.Instance.UpdateObjective(testObjectiveText);
        
        // 2. Update 3D Model
        ObjectiveUI.Instance.Show3DObjective(prefabToTest);
    }
}