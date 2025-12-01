using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [Header("Settings")]
    public int maxCount = 8;
    public float stackDelay = 0.25f;
    public float dropDelay = 0.25f;

    private List<GameObject> stackedItems = new List<GameObject>();
    private Queue<Func<GameObject>> pendingQueue = new Queue<Func<GameObject>>();
    private bool isStacking = false;
    private bool isDropping = false;

    public event Action<Func<GameObject>, int> OnItemAddedRequest; 
    public event Action<int> OnItemRemoved;                       


    public bool AddItem(GameObject prefab)
    {
        if (stackedItems.Count + pendingQueue.Count >= maxCount)
            return false;

        pendingQueue.Enqueue(() => prefab);
        if (!isStacking) StartCoroutine(StackRoutine());
        return true;
    }

    private IEnumerator StackRoutine()
    {
        isStacking = true;

        while (pendingQueue.Count > 0)
        {
            var factory = pendingQueue.Dequeue();
            int index = stackedItems.Count;

            OnItemAddedRequest?.Invoke(factory, index);

            yield return new WaitForSeconds(stackDelay);
        }

        isStacking = false;
    }

    public void RegisterVisual(GameObject visual)
    {
        stackedItems.Add(visual);
    }


    public void DropAll(IStackDropHandler handler)
    {
        if (!isDropping) StartCoroutine(DropRoutine(handler));
    }

    private IEnumerator DropRoutine(IStackDropHandler handler)
    {
        isDropping = true;

        while (stackedItems.Count > 0)
        {
            int lastIndex = stackedItems.Count - 1;
            var item = stackedItems[lastIndex];
            stackedItems.RemoveAt(lastIndex);

            OnItemRemoved?.Invoke(lastIndex);
            handler.OnDrop(item);

            yield return new WaitForSeconds(dropDelay);
        }

        isDropping = false;
    }

    public int GetCount() => stackedItems.Count;
}
