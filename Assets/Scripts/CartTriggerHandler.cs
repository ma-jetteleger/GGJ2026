using UnityEngine;

public class CartTriggerHandler : MonoBehaviour
{
    [SerializeField] private Cart cart;

    private void Awake()
    {
        if (cart == null)
        {
            cart = GetComponentInParent<Cart>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cart == null)
        {
            return;
        }

        cart.OnObjectDetected(other.gameObject);
    }
}
