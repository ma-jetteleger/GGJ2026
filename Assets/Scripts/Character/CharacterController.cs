using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private float lookTorqueStrength = 1f;

    private InputAction _mousePositionAction;
    private InputAction _mouseLookAction;
    private Camera _mainCamera;
    private Rigidbody _rigidbody;
    private float _pitch;

    private const string MousePositionLookActionName = "MousePosition";
    private const string LookActionName = "Look";

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        _mainCamera = Camera.main;
        _pitch = NormalizePitch(transform.rotation.eulerAngles.x);
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            return;
        }

        _mousePositionAction = playerInput.actions[MousePositionLookActionName];
        _mouseLookAction = playerInput.actions[LookActionName];
    }

    private void Update()
    {
        // CalculateCameraRotation();
        
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector2 lookDelta = _mouseLookAction != null ? _mouseLookAction.ReadValue<Vector2>() : Vector2.zero;
        if (lookDelta == Vector2.zero)
        {
            return;
        }

        _pitch = NormalizePitch(_rigidbody.rotation.eulerAngles.x);
        float targetPitch = Mathf.Clamp(_pitch - lookDelta.y, minPitch, maxPitch);
        float pitchDelta = targetPitch - _pitch;

        Vector3 torque = (transform.right * pitchDelta + Vector3.up * lookDelta.x) * lookTorqueStrength;
        _rigidbody.AddTorque(torque, ForceMode.VelocityChange);
    }

    private void CalculateCameraRotation()
    {
        Vector2 lookInput = _mousePositionAction != null ? _mousePositionAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 halfScreen = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 centered = lookInput - halfScreen;
        Vector2 normalized = new Vector2(
            halfScreen.x > 0f ? centered.x / halfScreen.x : 0f,
            halfScreen.y > 0f ? centered.y / halfScreen.y : 0f);
        Debug.Log(normalized);

        float yaw = normalized.x * 180f;
        float pitch = Mathf.Lerp(minPitch, maxPitch, (normalized.y + 1f) * 0.5f);
        Debug.Log(new Vector2(pitch, yaw));
        
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private static float NormalizePitch(float pitchDegrees)
    {
        if (pitchDegrees > 180f)
        {
            pitchDegrees -= 360f;
        }

        return pitchDegrees;
    }
}
