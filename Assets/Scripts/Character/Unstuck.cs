using UnityEngine;
using UnityEngine.InputSystem;

public class Unstuck : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string unstuckActionName = "Unstuck";
    [SerializeField] private Transform characterRoot;

    private InputAction _unstuckAction;

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (characterRoot == null)
        {
            characterRoot = transform.root;
        }

        if (playerInput != null)
        {
            _unstuckAction = playerInput.actions[unstuckActionName];
        }
    }

    private void OnEnable()
    {
        BindUnstuckAction();
    }

    private void OnDisable()
    {
        if (_unstuckAction != null)
        {
            _unstuckAction.performed -= HandleUnstuckPerformed;
        }
    }

    private void BindUnstuckAction()
    {
        if (playerInput == null)
        {
            return;
        }

        if (_unstuckAction == null)
        {
            _unstuckAction = playerInput.actions[unstuckActionName];
        }

        if (_unstuckAction != null)
        {
            _unstuckAction.performed += HandleUnstuckPerformed;
        }
    }

    private void HandleUnstuckPerformed(InputAction.CallbackContext context)
    {
        DetachGrippables();
        ResetCharacter();
    }

    private void DetachGrippables()
    {
        if (characterRoot == null)
        {
            return;
        }

        AttachGrippables[] grippers = characterRoot.GetComponentsInChildren<AttachGrippables>(true);
        for (int i = 0; i < grippers.Length; i++)
        {
            grippers[i].ForceDetach();
        }
    }

    private void ResetCharacter()
    {
        PlayerStart start = FindObjectOfType<PlayerStart>();
        if (start == null)
        {
            Debug.LogWarning("Unstuck: no PlayerStart found in the scene.");
            return;
        }

        if (characterRoot == null)
        {
            Debug.LogWarning("Unstuck: no character root set to reset.");
            return;
        }

        characterRoot.SetPositionAndRotation(start.transform.position, start.transform.rotation);
        
    }
}
