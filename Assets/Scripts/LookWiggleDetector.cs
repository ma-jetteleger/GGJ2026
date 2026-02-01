using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookWiggleDetector : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string lookActionName = "Look";
    [SerializeField] private float minInputMagnitude = 0.1f;
    [SerializeField] private float directionChangeThresholdDegrees = 90f;
    [SerializeField] private float stillTimeoutSeconds = 0.5f;
    [SerializeField] private float requiredMovingSeconds = 4f;
    [SerializeField] private bool debugLogging;
    [SerializeField] private bool debugGui;
    [SerializeField] private Rect debugGuiRect = new Rect(20f, 20f, 200f, 18f);

    private float _lastDirectionChangeTime = -999f;
    private float _movingTime;
    private bool _isMovingQuickly;
    private Vector2 _lastDirection;
    private bool _hasLastDirection;

    public bool IsMovingQuickly => _isMovingQuickly;
    public float MovingTime => _movingTime;
    public float WiggleProgress => requiredMovingSeconds > 0f ? Mathf.Clamp01(_movingTime / requiredMovingSeconds) : 0f;
    public event Action WiggledFully;

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    private void Update()
    {
        if (playerInput == null)
        {
            return;
        }

        Vector2 lookInput = playerInput.actions[lookActionName].ReadValue<Vector2>();
        float magnitude = lookInput.magnitude;
        bool hasInput = magnitude >= minInputMagnitude;
        float angleDelta = 0f;

        if (hasInput)
        {
            Vector2 currentDirection = lookInput / magnitude;
            if (_hasLastDirection)
            {
                angleDelta = Vector2.Angle(_lastDirection, currentDirection);
                if (angleDelta >= directionChangeThresholdDegrees)
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

        if (_isMovingQuickly && Time.time - _lastDirectionChangeTime > stillTimeoutSeconds)
        {
            _isMovingQuickly = false;
            _movingTime = 0f;
        }

        if (_isMovingQuickly)
        {
            _movingTime += Time.deltaTime;
            if (_movingTime >= requiredMovingSeconds)
            {
                Debug.Log("WE BE WIGGLED NOW!");
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
