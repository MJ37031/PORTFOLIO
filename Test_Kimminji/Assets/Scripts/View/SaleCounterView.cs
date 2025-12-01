using System.Collections;
using UnityEngine;

public class SaleCounterView : MonoBehaviour
{
    [Header("Grid Settings")]
    public Transform displayRoot;
    public int rows = 2;
    public int cols = 4;
    public float xSpacing = 0.5f;
    public float zSpacing = 0.5f;

    public void PlaceBread(GameObject bread, int index)
    {
        if (!displayRoot) displayRoot = transform;

        int row = index / cols;
        int col = index % cols;

        Vector3 localPos = new Vector3(col * xSpacing, 0, row * zSpacing);

        bread.transform.SetParent(displayRoot);
        bread.transform.localPosition = localPos;
        bread.transform.localRotation = Quaternion.identity;

        StartCoroutine(PopAnimation(bread.transform));
    }

    public GameObject TakeLastBread()
    {
        if (!displayRoot || displayRoot.childCount == 0) return null;

        int lastIndex = displayRoot.childCount - 1;
        Transform last = displayRoot.GetChild(lastIndex);

        GameObject bread = last.gameObject;
        bread.transform.SetParent(null); 
        return bread;
    }

    private IEnumerator PopAnimation(Transform targetTransform)
    {
        Vector3 startPos = targetTransform.localPosition + Vector3.up * 0.5f;

        Vector3 endPos = targetTransform.localPosition;

        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float curveValue = 1 - Mathf.Pow(1 - t, 2);

            targetTransform.localPosition = Vector3.Lerp(startPos, endPos, curveValue);

            yield return null;
        }

        targetTransform.localPosition = endPos;
    }
}