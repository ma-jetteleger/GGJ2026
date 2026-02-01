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
    private List<ObjectiveViewResult> _createdViews = new List<ObjectiveViewResult>();
    private Dictionary<string, GameObject> _itemPrefabCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (groceryListPanel != null)
        {
            groceryListPanel.SetActive(isOpen);
        }
        PopulateList();
    }

    private void OnDestroy()
    {
        ClearViews();
    }

    private void ClearViews()
    {
        foreach (var view in _createdViews)
        {
            view.Destroy();
        }
        _createdViews.Clear();
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

    public void AddItem(string itemName, GameObject prefab = null)
    {
        if (!groceryItems.Contains(itemName))
        {
            groceryItems.Add(itemName);
            if (prefab != null)
            {
                _itemPrefabCache[itemName] = prefab;
            }
            CreateListItem(itemName, prefab);
        }
    }

    public void MarkItemFound(string itemName)
    {
        if (itemsContainer == null) return;
        
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
        ClearViews();
        _itemPrefabCache.Clear();
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
        ClearViews(); // Clean up old views if repopulating

        if (itemsContainer == null || itemPrefab == null) return;

        // Clear existing items UI
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Add new items
        foreach (string itemName in groceryItems)
        {
            // Try to find cached prefab
            GameObject prefab = null;
            if (_itemPrefabCache.ContainsKey(itemName))
            {
                prefab = _itemPrefabCache[itemName];
            }
            CreateListItem(itemName, prefab);
        }
    }

    private void CreateListItem(string itemName, GameObject targetPrefab)
    {
        GameObject newItem = Instantiate(itemPrefab, itemsContainer);
        GroceryListItem script = newItem.GetComponent<GroceryListItem>();
        
        if (script != null)
        {
            Texture icon = null;
            
            // Only create view if we have a valid prefab
            if (targetPrefab != null)
            {
                if (ObjectiveViewFactory.Instance != null)
                {
                    var result = ObjectiveViewFactory.Instance.CreateView(targetPrefab);
                    if (result != null)
                    {
                        _createdViews.Add(result);
                        icon = result.Texture;
                    }
                }
                else
                {
                    Debug.LogWarning("[GroceryListController] ObjectiveViewFactory Instance is null. Icons will not be generated.");
                }
            }

            script.Setup(itemName, icon);
        }
    }
}