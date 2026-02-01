using System.Collections.Generic;
using UnityEngine;

public class CartManager : MonoBehaviour
{
    [SerializeField] private List<Cart> carts = new List<Cart>();
    [SerializeField] private PrefabLibrary prefabLibrary = null;

    private void Awake()
    {
        foreach (Cart cart in carts)
        {
            if (cart != null)
            {
                cart.prefabLibrary = prefabLibrary;
            }
        }
    }

    private void OnEnable()
    {
        foreach (Cart cart in carts)
        {
            if (cart != null)
            {
                cart.Completed += HandleCartCompleted;
            }
        }
    }

    private void OnDisable()
    {
        foreach (Cart cart in carts)
        {
            if (cart != null)
            {
                cart.Completed -= HandleCartCompleted;
            }
        }
    }

    private void HandleCartCompleted(Cart cart)
    {
        if (carts == null || carts.Count == 0)
        {
            return;
        }

        foreach (Cart listedCart in carts)
        {
            if (listedCart == null)
            {
                continue;
            }

            if (!listedCart.IsComplete)
            {
                return;
            }
        }

        foreach (Cart listedCart in carts)
        {
            if (listedCart != null)
            {
                listedCart.ResetCart();
            }
        }
    }
}