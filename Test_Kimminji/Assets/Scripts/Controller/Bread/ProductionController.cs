using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionController : MonoBehaviour
{
    public BreadData breadData;

    [Header("Spawn Points")]
    public Transform initialSpawnPoint;  // 시작 시 빵 배치
    public Transform bakeSpawnPoint;     // 이후 자동 생산 시

    public int maxBread = 8;
    public SaleCounterController saleCounter;

    private bool isProducing = false;
    private List<GameObject> producedBreads = new List<GameObject>();

    private void Start()
    {
        // 시작 시 8개 초기 세팅 → initialSpawnPoint 기준
        for (int i = 0; i < maxBread; i++)
        {
            SpawnOvenBread(breadData, initialSpawnPoint, i);
        }

        StartCoroutine(AutoProductionLoop());
    }

    private Vector3 GetOvenOffset(int index)
    {
        int row = index / 4;   // 위아래 2줄
        int col = index % 4;   // 가로 4칸

        float xSpacing = 0.2f;
        float zSpacing = 0.2f;

        return new Vector3(col * xSpacing, 0, row * zSpacing);
    }

    private void SpawnOvenBread(BreadData breadData, Transform point, int index)
    {
        Vector3 pos = point.position + GetOvenOffset(index);
        GameObject bread = Instantiate(breadData.prefab, pos, Quaternion.identity);
        producedBreads.Add(bread);
    }

    private IEnumerator AutoProductionLoop()
    {
        while (true)
        {
            if (producedBreads.Count < maxBread && !isProducing)
            {
                yield return StartCoroutine(ProductionCoroutine(breadData));
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator ProductionCoroutine(BreadData breadData)
    {
        isProducing = true;

        float timer = 0f;
        while (timer < breadData.productionTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 자동 생산 → bakeSpawnPoint 사용
        yield return StartCoroutine(BakeBread(breadData.prefab, bakeSpawnPoint));

        isProducing = false;
    }

    private IEnumerator BakeBread(GameObject breadPrefab, Transform point)
    {
        Vector3 startPos = point.position - point.forward * 0.2f;
        Vector3 targetPos = point.position + point.forward * 0.8f;

        GameObject bread = Instantiate(breadPrefab, startPos, Quaternion.identity);

        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f); // EaseOut
            bread.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        bread.transform.position = targetPos;

        producedBreads.Add(bread);
    }

    public GameObject TakeBread()
    {
        if (producedBreads.Count == 0) return null;
        GameObject bread = producedBreads[0];
        producedBreads.RemoveAt(0);
        return bread;
    }

    public void RemoveBread(GameObject bread)
    {
        if (producedBreads.Contains(bread))
        {
            producedBreads.Remove(bread);
            Debug.Log("오븐 리스트에서 빵 제거됨, 현재 개수: " + producedBreads.Count);
        }
    }
}
