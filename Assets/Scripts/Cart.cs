using System;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [SerializeField] private List<GameObject> requirementPrefabs = new List<GameObject>();
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject celebrationVfx;
    [SerializeField] private Vector3 celebrationOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private int targetScore = 1;
    [SerializeField] private float indicatorHoverAmplitude = 0.15f;
    [SerializeField] private float indicatorHoverSpeed = 1f;
    [SerializeField] private float indicatorRotationSpeed = 45f;
    [SerializeField] private GameObject indicatorGlow = null;
    [SerializeField] private ParticleSystem winAnimation = null;
    [SerializeField] private GameObject fireAnimation = null;

    private GameObject requiredPrefab;
    private GameObject currentIndicator;
    private Vector3 indicatorBaseLocalPosition;
    private Quaternion indicatorBaseLocalRotation;
    private int scoreCounter;

    private void Start()
    {
        SpawnRequirement();
    }

    public GameObject PickNewRequirement()
    {
        if (requirementPrefabs == null || requirementPrefabs.Count == 0)
        {
            return null;
        }

        return requirementPrefabs[UnityEngine.Random.Range(0, requirementPrefabs.Count)];
    }

    public void SpawnRequirement()
    {
        requiredPrefab = PickNewRequirement();

        if (requiredPrefab == null || targetTransform == null)
        {
            return;
        }

        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }

        currentIndicator = Instantiate(requiredPrefab, targetTransform);
        currentIndicator.transform.localPosition = Vector3.zero;
        currentIndicator.transform.localRotation = Quaternion.identity;
        indicatorBaseLocalPosition = currentIndicator.transform.localPosition;
        indicatorBaseLocalRotation = currentIndicator.transform.localRotation;
        RemovePhysicsComponents(currentIndicator);
        
        Debug.Log($"Picked new requirement prefab: {requiredPrefab.name}");

        if(!indicatorGlow.activeSelf)
        {
			indicatorGlow.SetActive(true);
		}
	}

    public void OnObjectDetected(GameObject detectedObject)
    {
        if (scoreCounter >= targetScore)
        {
            return;
        }

        if (requiredPrefab == null || detectedObject == null)
        {
            return;
        }

        if (detectedObject.GetComponent<PrefabGuid>() == null)
        {
            return;
        }

        if (detectedObject.GetComponent<HeldByPlayerTag>() != null)
        {
            return;
        }
        
        Debug.Log($"Object detected: {detectedObject.name}");

        if (!IsMatch(detectedObject, requiredPrefab))
        {
            fireAnimation.SetActive(true);
            fireAnimation.GetComponent<ParticleSystem>().Play();

			Invoke("StopFireAnimation", 3f);

			return;
        }
        
        Debug.Log($"Got a match for {detectedObject.name}");

        if (celebrationVfx != null)
        {
            Instantiate(celebrationVfx, transform.position + celebrationOffset, Quaternion.identity);
        }

        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }

        Debug.Log($"Spawned indicator for {detectedObject.name}");

        scoreCounter++;

        AudioEventManager.Instance.PlaySoundEvent("score", transform.position);
        
        winAnimation.Play();

		Debug.Log($"Score: {scoreCounter}");
        if (scoreCounter < targetScore)
        {
            SpawnRequirement();
        }
        else
        {
			indicatorGlow.SetActive(false);
		}
    }

    private static bool IsMatch(GameObject detectedObject, GameObject prefab)
    {
        if (detectedObject == prefab)
        {
            return true;
        }

        var detectedGuidComponent = detectedObject.GetComponent<PrefabGuid>();
        var prefabGuidComponent = prefab.GetComponent<PrefabGuid>();

        
        if (detectedGuidComponent == null || prefabGuidComponent == null)
        {
            Debug.LogError($"{prefab.name} or {detectedObject.name} does not have a PrefabGuid component!");
            return false;
        }
        
        Debug.Log($"Comparing {detectedObject} [{detectedGuidComponent.Guid}] to {prefab} [{prefabGuidComponent.Guid}]");

        return !string.IsNullOrEmpty(detectedGuidComponent.Guid)
               && string.Equals(detectedGuidComponent.Guid, prefabGuidComponent.Guid, StringComparison.Ordinal);
    }

    private void Update()
    {
        if (currentIndicator == null)
        {
            return;
        }

        var offsetY = Mathf.Sin(Time.time * indicatorHoverSpeed) * indicatorHoverAmplitude;
        currentIndicator.transform.localPosition = indicatorBaseLocalPosition + new Vector3(0f, offsetY, 0f);
        currentIndicator.transform.localRotation = indicatorBaseLocalRotation
            * Quaternion.Euler(0f, Time.time * indicatorRotationSpeed, 0f);
    }

    private static void RemovePhysicsComponents(GameObject indicator)
    {
        foreach (var collider in indicator.GetComponentsInChildren<Collider>(true))
        {
            Destroy(collider);
        }

        foreach (var rigidbody in indicator.GetComponentsInChildren<Rigidbody>(true))
        {
            Destroy(rigidbody);
        }
    }

    private void StopFireAnimation()
    {
		fireAnimation.SetActive(false);
	}
}
