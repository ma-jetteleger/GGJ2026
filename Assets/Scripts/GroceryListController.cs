using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GroceryListController : MonoBehaviour
{
    [SerializeField] private GameObject groceryListPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private List<string> groceryItems = new List<string> { "Apples", "Milk", "Bread", "Eggs", "Cheese" };

    private bool isOpen = false;

    private void Start()
    {
        if (groceryListPanel != null)
        {
            groceryListPanel.SetActive(isOpen);
        }
        PopulateList();
    }

    public void OnList(InputValue value)
    {
        if (value.isPressed)
        {
            ToggleList();
        }
    }

    public void ToggleList()
    {
        ShowList(!isOpen);
    }

    public void ShowList(bool show)
    {
        isOpen = show;
        if (groceryListPanel != null)
        {
            groceryListPanel.SetActive(isOpen);
        }
    }

    public void AddItem(string itemName)
    {
        if (!groceryItems.Contains(itemName))
        {
            groceryItems.Add(itemName);
            
            if (itemsContainer != null && itemPrefab != null)
            {
                GameObject newItem = Instantiate(itemPrefab, itemsContainer);
                GroceryListItem script = newItem.GetComponent<GroceryListItem>();
                if (script != null)
                {
                    script.Setup(itemName);
                }
            }
        }
    }

    public void MarkItemFound(string itemName)
    {
        if (itemsContainer == null) return;

        foreach (Transform child in itemsContainer)
        {
            GroceryListItem item = child.GetComponent<GroceryListItem>();
            // We assume the label text is the item name. 
            // A cleaner way would be to have a public getter for the name in GroceryListItem.
            // For now, we rely on the visual text or assume strict order, but text is safer.
            // Let's assume the controller manages the logic, or we just scan components.
            // Actually, we need to inspect the script's internal state or expose the name.
            // For simplicity, let's look for a match in the text component if we can access it, 
            // or better yet, update GroceryListItem to store its name publically.
        }
        
        // Revised approach: iterate and check text.
        GroceryListItem[] items = itemsContainer.GetComponentsInChildren<GroceryListItem>();
        foreach (var item in items)
        {
            if (item.GetItemName() == itemName)
            {
                item.SetChecked(true);
                break;
            }
        }
    }

    public void ClearList()
    {
        if (itemsContainer != null)
        {
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }
        }
        groceryItems.Clear();
    }

    private void PopulateList()
    {
        if (itemsContainer == null || itemPrefab == null) return;

        // Clear existing items
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Add new items
        foreach (string itemName in groceryItems)
        {
            GameObject newItem = Instantiate(itemPrefab, itemsContainer);
            GroceryListItem script = newItem.GetComponent<GroceryListItem>();
            if (script != null)
            {
                script.Setup(itemName);
            }
        }
    }
}