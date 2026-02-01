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

    private GameObject requiredPrefab;
    private GameObject currentIndicator;
    private Vector3 indicatorBaseLocalPosition;
    private int scoreCounter;

    private void Start()
    {
        PickNewRequirement();
        SpawnRequirement();
    }

    public void PickNewRequirement()
    {
        if (requirementPrefabs == null || requirementPrefabs.Count == 0)
        {
            requiredPrefab = null;
            return;
        }

        requiredPrefab = requirementPrefabs[UnityEngine.Random.Range(0, requirementPrefabs.Count)];
    }

    public void SpawnRequirement()
    {
        if (requiredPrefab == null)
        {
            PickNewRequirement();
        }

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
        RemovePhysicsComponents(currentIndicator);
    }

    public void OnObjectDetected(GameObject detectedObject)
    {
        if (requiredPrefab == null || detectedObject == null)
        {
            return;
        }

        if (!IsMatch(detectedObject, requiredPrefab))
        {
            return;
        }

        if (celebrationVfx != null)
        {
            Instantiate(celebrationVfx, transform.position + celebrationOffset, Quaternion.identity);
        }

        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }

        scoreCounter++;
        if (scoreCounter < targetScore)
        {
            PickNewRequirement();
            SpawnRequirement();
        }
    }

    private static bool IsMatch(GameObject detectedObject, GameObject prefab)
    {
        if (detectedObject == prefab)
        {
            return true;
        }

        var detectedName = detectedObject.name;
        var prefabName = prefab.name;
        return string.Equals(detectedName, prefabName, StringComparison.Ordinal)
            || string.Equals(detectedName, prefabName + "(Clone)", StringComparison.Ordinal);
    }

    private void Update()
    {
        if (currentIndicator == null)
        {
            return;
        }

        var offsetY = Mathf.Sin(Time.time * indicatorHoverSpeed) * indicatorHoverAmplitude;
        currentIndicator.transform.localPosition = indicatorBaseLocalPosition + new Vector3(0f, offsetY, 0f);
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
}
