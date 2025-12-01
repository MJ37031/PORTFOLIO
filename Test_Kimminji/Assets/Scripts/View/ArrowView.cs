using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ArrowView : MonoBehaviour
{
    public static ArrowView I;
    public GameObject arrowPrefab;
    public float heightOffset = 1.5f;

    private GameObject arrowInstance;
    private (Transform target, int priority) current;
    private List<(Transform target, int priority)> activeEvents = new List<(Transform, int)>();

    private Transform currentTarget;

    void Awake()
    {
        I = this;
    }

    public void ShowArrow(Transform target, int priority)
    {
        if (target == null || arrowPrefab == null) return;

        int idx = activeEvents.FindIndex(e => e.target == target);
        if (idx >= 0)
        {
            activeEvents[idx] = (target, priority);
        }
        else
        {
            activeEvents.Add((target, priority));
        }

        UpdateArrow();
    }
    public void HideArrow(Transform target)
    {
        activeEvents.RemoveAll(e => e.target == target);
        UpdateArrow();
    }

    private void UpdateArrow()
    {
        if (activeEvents.Count == 0)
        {
            if (arrowInstance != null) arrowInstance.SetActive(false);
            current = (null, -1);
            return;
        }

        var highest = activeEvents.OrderByDescending(e => e.priority).First();

        if (arrowInstance == null)
            arrowInstance = Instantiate(arrowPrefab);

        if (highest.target != current.target)
        {
            current = highest;
            arrowInstance.transform.SetParent(highest.target);
            arrowInstance.transform.localPosition = Vector3.up * heightOffset;
            arrowInstance.transform.localRotation = Quaternion.identity;
        }

        arrowInstance.SetActive(true);
    }

    public Transform GetCurrentTargetTransform()
    {
        if (current.target != null && arrowInstance != null && arrowInstance.activeSelf)
        {
            return current.target;
        }
        return null;
    }
}