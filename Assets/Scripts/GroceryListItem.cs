using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GroceryListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemLabel;
    [SerializeField] private Image checkmarkImage;
    [SerializeField] private Button button;

    private string itemName;
    private bool isChecked = false;

    private void Start()
    {
        UpdateVisuals();
    }

    public void Setup(string name)
    {
        this.itemName = name;
        if (itemLabel != null) itemLabel.text = name;
        isChecked = false;
        UpdateVisuals();
    }

    public string GetItemName()
    {
        return itemName;
    }

    public void SetChecked(bool state)
    {
        isChecked = state;
        UpdateVisuals();
    }

    public bool IsChecked()
    {
        return isChecked;
    }

    private void UpdateVisuals()
    {
        if (checkmarkImage != null)
        {
            checkmarkImage.enabled = isChecked;
        }
    }
}
