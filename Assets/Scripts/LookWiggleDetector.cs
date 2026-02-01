using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookWiggleDetector : MonoBehaviour
{
    [Serializable]
    public struct LookWiggleSettings
    {
        public float minInputMagnitude;
        public float directionChangeThresholdDegrees;
        public float stillTimeoutSeconds;
        public float requiredMovingSeconds;
    }

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string lookActionName = "Look";
    [SerializeField] private LookWiggleSettings mouseSettings;
    [SerializeField] private LookWiggleSettings controllerSettings;
    [SerializeField] private bool debugLogging;
    [SerializeField] private bool debugGui;
    [SerializeField] private Rect debugGuiRect = new Rect(20f, 20f, 200f, 18f);

    private InputAction _lookAction;
    private LookWiggleSettings _activeSettings;
    private float _lastDirectionChangeTime = -999f;
    private float _movingTime;
    private bool _isMovingQuickly;
    private Vector2 _lastDirection;
    private bool _hasLastDirection;

    public bool IsMovingQuickly => _isMovingQuickly;
    public float MovingTime => _movingTime;
    public float WiggleProgress => _activeSettings.requiredMovingSeconds > 0f
        ? Mathf.Clamp01(_movingTime / _activeSettings.requiredMovingSeconds)
        : 0f;
    public event Action WiggledFully;

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (playerInput != null)
        {
            _lookAction = playerInput.actions[lookActionName];
        }

        _activeSettings = mouseSettings;
    }

    private void Update()
    {
        if (playerInput == null)
        {
            return;
        }

        if (_lookAction == null)
        {
            _lookAction = playerInput.actions[lookActionName];
            if (_lookAction == null)
            {
                return;
            }
        }

        _activeSettings = GetActiveSettings();
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();
        float magnitude = lookInput.magnitude;
        bool hasInput = magnitude >= _activeSettings.minInputMagnitude;
        float angleDelta = 0f;

        if (hasInput)
        {
            Vector2 currentDirection = lookInput / magnitude;
            if (_hasLastDirection)
            {
                angleDelta = Vector2.Angle(_lastDirection, currentDirection);
                if (angleDelta >= _activeSettings.directionChangeThresholdDegrees)
                {
                    _lastDirectionChangeTime = Time.time;
                    if (!_isMovingQuickly)
                    {
                        _isMovingQuickly = true;
                        _movingTime = 0f;
                    }
                }
            }

            _lastDirection = currentDirection;
            _hasLastDirection = true;
        }

        if (_isMovingQuickly && Time.time - _lastDirectionChangeTime > _activeSettings.stillTimeoutSeconds)
        {
            _isMovingQuickly = false;
            _movingTime = 0f;
        }

        if (_isMovingQuickly)
        {
            _movingTime += Time.deltaTime;
            if (_movingTime >= _activeSettings.requiredMovingSeconds)
            {
                Debug.Log("<color=green>WE BE WIGGLED NOW!</color>");
                WiggledFully?.Invoke();
                ResetWiggle();
            }
        }

        if (debugLogging)
        {
            float sinceChange = Time.time - _lastDirectionChangeTime;
            Debug.Log($"Look mag={magnitude:0.000} angle={angleDelta:0.0} moving={_isMovingQuickly} timer={_movingTime:0.00}s sinceChange={sinceChange:0.00}s");
        }
    }

    public void ResetWiggle()
    {
        _lastDirectionChangeTime = -999f;
        _movingTime = 0f;
        _isMovingQuickly = false;
        _lastDirection = Vector2.zero;
        _hasLastDirection = false;
    }

    private LookWiggleSettings GetActiveSettings()
    {
        InputDevice activeDevice = _lookAction.activeControl?.device;
        if (activeDevice is Mouse)
        {
            return mouseSettings;
        }

        if (activeDevice != null)
        {
            return controllerSettings;
        }

        string scheme = playerInput.currentControlScheme;
        if (!string.IsNullOrEmpty(scheme) && scheme.IndexOf("Mouse", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return mouseSettings;
        }

        return controllerSettings;
    }

    private void OnGUI()
    {
        if (!debugGui)
            return;

        float progress = WiggleProgress;
        GUI.Box(debugGuiRect, string.Empty);
        Rect fillRect = new Rect(debugGuiRect.x, debugGuiRect.y, debugGuiRect.width * progress, debugGuiRect.height);
        GUI.Box(fillRect, $"Wiggle {progress:P0}");
    }
}
