using ActiveRagdoll;
using System;
using UnityEngine;

public class AttachGrippables : MonoBehaviour
{
    [SerializeField] private float detachCollisionCooldownSeconds = 0.5f;
    [SerializeField] private ParticleSystem attachAnimation = null;
    [SerializeField] private ParticleSystem detachAnimation = null;
    
    public event Action OnAttach;
    public event Action OnDetach;

    private struct RigidbodySettings
    {
        public float Mass;
        public float Drag;
        public float AngularDrag;
        public RigidbodyInterpolation Interpolation;
        public CollisionDetectionMode CollisionDetectionMode;
        public RigidbodyConstraints Constraints;
    }

    private GameObject _grabbedObject;
    private Transform _originalParent;
    private bool _originalUseGravity;
    private bool _originalIsKinematic;
    private RigidbodySettings _originalRigidbodySettings;
    private LookWiggleDetector _lookWiggleDetector;
    private float _nextCollisionTime;

    private bool CanGrip()
    {
        return _grabbedObject == null;
    }

    private void Awake()
    {
        _lookWiggleDetector = transform.root.GetComponent<LookWiggleDetector>();
    }

    private void OnEnable()
    {
        if (_lookWiggleDetector != null)
        {
            _lookWiggleDetector.WiggledFully += HandleWiggleFully;
        }
    }

    private void OnDisable()
    {
        if (_lookWiggleDetector != null)
        {
            _lookWiggleDetector.WiggledFully -= HandleWiggleFully;
        }
    }

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
        if (Time.time < _nextCollisionTime)
            return;

        if (!CanGrip())
            return;
        
        if (targetRigidbody == null || targetRigidbody.transform.IsChildOf(transform.root))
            return;

		if (targetRigidbody != null && !targetRigidbody.isKinematic)
        {
            GameObject attachGrippableGameObject = targetRigidbody.gameObject;
            _grabbedObject = attachGrippableGameObject;
            _originalParent = targetRigidbody.transform.parent;
            _originalUseGravity = targetRigidbody.useGravity;
            _originalIsKinematic = targetRigidbody.isKinematic;
            _originalRigidbodySettings = new RigidbodySettings
            {
                Mass = targetRigidbody.mass,
                Drag = targetRigidbody.linearDamping,
                AngularDrag = targetRigidbody.angularDamping,
                Interpolation = targetRigidbody.interpolation,
                CollisionDetectionMode = targetRigidbody.collisionDetectionMode,
                Constraints = targetRigidbody.constraints
            };

            targetRigidbody.transform.SetParent(transform);
            targetRigidbody.useGravity = false;
            targetRigidbody.isKinematic = true;
            targetRigidbody.transform.position = targetRigidbody.transform.position - (targetRigidbody.transform.position - transform.position).normalized * 0.1f;

            if (!_grabbedObject.TryGetComponent<HeldByPlayerTag>(out _))
            {
                _grabbedObject.AddComponent<HeldByPlayerTag>();
            }

            Collider targetCollider = targetRigidbody.GetComponent<Collider>();
            if (targetCollider != null)
            {
                Destroy(targetRigidbody);
            }

            attachAnimation.Play();
		}

        OnAttach?.Invoke();
    }

    private void HandleWiggleFully()
    {
        DetachGrabbed();
    }

    public void ForceDetach()
    {
        DetachGrabbed();
    }

    private void DetachGrabbed()
    {
        if (_grabbedObject == null)
            return;

        Rigidbody targetRigidbody = _grabbedObject.GetComponent<Rigidbody>();
        if (targetRigidbody == null)
        {
            targetRigidbody = _grabbedObject.AddComponent<Rigidbody>();
            targetRigidbody.mass = _originalRigidbodySettings.Mass;
            targetRigidbody.linearDamping = _originalRigidbodySettings.Drag;
            targetRigidbody.angularDamping = _originalRigidbodySettings.AngularDrag;
            targetRigidbody.interpolation = _originalRigidbodySettings.Interpolation;
            targetRigidbody.collisionDetectionMode = _originalRigidbodySettings.CollisionDetectionMode;
            targetRigidbody.constraints = _originalRigidbodySettings.Constraints;
        }

        targetRigidbody.useGravity = _originalUseGravity;
        targetRigidbody.isKinematic = _originalIsKinematic;

        _grabbedObject.transform.SetParent(_originalParent, true);

        var heldTag = _grabbedObject.GetComponent<HeldByPlayerTag>();
        if (heldTag != null)
        {
            Destroy(heldTag);
        }

        _grabbedObject = null;
        _originalParent = null;
        _nextCollisionTime = Time.time + detachCollisionCooldownSeconds;

		detachAnimation.Play();

		OnDetach?.Invoke();
    }
}
