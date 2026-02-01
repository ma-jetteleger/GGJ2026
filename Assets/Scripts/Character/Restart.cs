using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string restartActionName = "Restart";

    private InputAction _restartAction;

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (playerInput != null)
        {
            _restartAction = playerInput.actions[restartActionName];
        }
    }

    private void OnEnable()
    {
        BindRestartAction();
    }

    private void OnDisable()
    {
        if (_restartAction != null)
        {
            _restartAction.performed -= HandleRestartPerformed;
        }
    }

    private void BindRestartAction()
    {
        if (playerInput == null)
        {
            return;
        }

        if (_restartAction == null)
        {
            _restartAction = playerInput.actions[restartActionName];
        }

        if (_restartAction != null)
        {
            _restartAction.performed += HandleRestartPerformed;
        }
    }

    private void HandleRestartPerformed(InputAction.CallbackContext context)
    {
        RestartScene();
    }

    private void RestartScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid())
        {
            Debug.LogWarning("Restart: no active scene found to reload.");
            return;
        }

        SceneManager.LoadScene(activeScene.buildIndex, LoadSceneMode.Single);
    }
}
