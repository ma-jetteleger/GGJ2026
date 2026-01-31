using ActiveRagdoll;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttachGrippables : MonoBehaviour
{
    public event Action OnAttach;
    public event Action OnDetach;

    private List<AttachGrippables> _attachedGrippables;

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.rigidbody);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.attachedRigidbody);
    }

    private void HandleCollision(Rigidbody targetRigidbody)
    {
        if (targetRigidbody == null || targetRigidbody.transform.IsChildOf(transform.root))
            return;

        var targetGrippable = targetRigidbody?.GetComponent<Grippable>();

        if(targetGrippable == null || targetGrippable.Gripped)
        {
			return;
		}

		if (targetRigidbody != null && !targetRigidbody.isKinematic)
        {
            targetGrippable.transform.SetParent(transform);
            targetRigidbody.useGravity = false;
            targetRigidbody.isKinematic = true;
            targetRigidbody.transform.position = targetRigidbody.transform.position - (targetRigidbody.transform.position - transform.position).normalized * 0.1f;

            Collider targetCollider = targetRigidbody.GetComponent<Collider>();
            if (targetCollider != null)
            {
                // component.isTrigger = true;
                targetCollider.gameObject.layer = LayerMask.NameToLayer("GrippedGrippables");
            }

            targetGrippable.Gripped = true;

			var newAttachGrippable = targetRigidbody.AddComponent<AttachGrippables>();
            _attachedGrippables.Add(newAttachGrippable);
		}

        OnAttach?.Invoke();
    }
}
