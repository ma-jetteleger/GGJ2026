using UnityEngine;

public class ObjectiveTester : MonoBehaviour
{
    [Header("Test Settings")]
    public GameObject prefabToTest;
    public string testObjectiveText = "Test Objective";

    // This method is called by the custom button in the Inspector
    public void TestDisplay()
    {
        if (prefabToTest == null)
        {
            Debug.LogWarning("Assign a prefab to test first!");
            return;
        }

        GroceryListController controller = FindObjectOfType<GroceryListController>();
        if (controller == null)
        {
            Debug.LogError("GroceryListController not found in the scene!");
            return;
        }

        // Add the item to the grocery list
        // Note: Duplicate items with the same name will be ignored by the controller's logic
        controller.AddItem(testObjectiveText, prefabToTest);
    }
}