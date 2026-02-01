using UnityEngine;
using UnityEngine.InputSystem;

public class UnstuckRespawn : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string unstuckActionName = "Unstuck";
    [SerializeField] private Transform characterRoot;
    [SerializeField] private GameObject characterPrefab;

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
        RespawnCharacter();
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

    private void RespawnCharacter()
    {
        PlayerStart start = FindObjectOfType<PlayerStart>();
        if (start == null)
        {
            Debug.LogWarning("UnstuckRespawn: no PlayerStart found in the scene.");
            return;
        }

        if (characterRoot == null)
        {
            Debug.LogWarning("UnstuckRespawn: no character root set to destroy.");
            return;
        }

        Transform parent = characterRoot.parent;
        GameObject prefabToSpawn = characterPrefab != null ? characterPrefab : characterRoot.gameObject;

        Instantiate(prefabToSpawn, start.transform.position, start.transform.rotation, parent);
        Destroy(characterRoot.gameObject);
    }
}
