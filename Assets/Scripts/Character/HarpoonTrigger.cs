using System;
using UnityEngine;

public class HarpoonTrigger : MonoBehaviour
{
    public event Action Pinned;
    public event Action Unpinned;

    [Header("Pinning")]
    [SerializeField] private bool _autoPin = true;
    [SerializeField] private bool _allowSelfPin = false;
    [SerializeField] private float _breakForce = Mathf.Infinity;
    [SerializeField] private float _breakTorque = Mathf.Infinity;
    [SerializeField] private float _tetherDistance = 0.5f;
    [SerializeField] private float _tetherSpring = 500f;
    [SerializeField] private float _tetherDamper = 50f;
    [SerializeField] private Rigidbody _pinSourceBody;

    private Joint _joint;
    private Rigidbody _connectedBody;
    private GameObject _anchorObject;

    private void Awake()
    {
        if (_pinSourceBody == null)
            _pinSourceBody = GetComponentInParent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_autoPin)
            TryPin(collision.rigidbody, GetContactPoint(collision));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_autoPin)
            TryPin(other.attachedRigidbody, other.ClosestPoint(transform.position));
    }

    public void Unpin()
    {
        if (_joint == null)
            return;

        Destroy(_joint);

        if (_anchorObject != null)
            Destroy(_anchorObject);

        _joint = null;
        _connectedBody = null;
        _anchorObject = null;
        Unpinned?.Invoke();
    }

    private void OnDisable()
    {
        Unpin();
    }

    private void OnJointBreak(float breakForce)
    {
        Unpin();
    }

    private void TryPin(Rigidbody targetBody, Vector3 worldPoint)
    {
        if (_joint != null)
            return;

        if (_pinSourceBody == null)
        {
            Debug.LogWarning("HarpoonTrigger needs a pin source Rigidbody (usually head).", this);
            return;
        }

        if (!_allowSelfPin && targetBody != null && targetBody.transform.IsChildOf(transform.root))
            return;

        if (targetBody == _pinSourceBody)
            return;

        if (targetBody != null && !targetBody.isKinematic)
            return;

        _connectedBody = targetBody != null ? targetBody : CreateWorldAnchor(worldPoint);

        _joint = _tetherDistance > 0f
            ? CreateTetherJoint(_pinSourceBody, _connectedBody, worldPoint)
            : CreateFixedJoint(_pinSourceBody, _connectedBody, worldPoint);
        Debug.Log("Harpoon pinned.", this);
        Pinned?.Invoke();
    }

    private Vector3 GetContactPoint(Collision collision)
    {
        if (collision.contactCount > 0)
            return collision.GetContact(0).point;

        return transform.position;
    }

    private Rigidbody CreateWorldAnchor(Vector3 worldPoint)
    {
        _anchorObject = new GameObject("HarpoonAnchor");
        _anchorObject.transform.position = worldPoint;
        _anchorObject.transform.rotation = Quaternion.identity;

        var anchorBody = _anchorObject.AddComponent<Rigidbody>();
        anchorBody.isKinematic = true;
        return anchorBody;
    }

    private Joint CreateFixedJoint(Rigidbody sourceBody, Rigidbody targetBody, Vector3 worldPoint)
    {
        var joint = sourceBody.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = targetBody;
        joint.enableCollision = false;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = sourceBody.transform.InverseTransformPoint(worldPoint);
        joint.connectedAnchor = targetBody.transform.InverseTransformPoint(worldPoint);
        joint.breakForce = _breakForce;
        joint.breakTorque = _breakTorque;
        return joint;
    }

    private Joint CreateTetherJoint(Rigidbody sourceBody, Rigidbody targetBody, Vector3 worldPoint)
    {
        var joint = sourceBody.gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = targetBody;
        joint.enableCollision = false;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = sourceBody.transform.InverseTransformPoint(worldPoint);
        joint.connectedAnchor = targetBody.transform.InverseTransformPoint(worldPoint);
        joint.breakForce = _breakForce;
        joint.breakTorque = _breakTorque;

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        joint.linearLimit = new SoftJointLimit { limit = _tetherDistance };
        joint.linearLimitSpring = new SoftJointLimitSpring {
            spring = _tetherSpring,
            damper = _tetherDamper
        };

        return joint;
    }
}
