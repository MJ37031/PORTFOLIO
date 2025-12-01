using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
public class MoneyStacker : MonoBehaviour
{

    [Header("Money Settings")]
    public GameObject moneyPrefab;   // 생성할 돈 프리팹
    public Transform stackPoint;     // 돈이 쌓일 기준 위치
    public int maxStack = 200;       // 최대 쌓이는 개수
    public int moneyValue = 1;       // 돈 1개당 가치
    public AudioSource putmoneySource;
    public AudioSource getmoneySource;

    [Header("Offsets")]
    public float xOffset = 1.5f;     // 가로 간격
    public float zOffset = 1.5f;     // 앞뒤 간격
    public float yOffset = 0.5f;     // 층 높이 간격

    [Header("Grid Size")]
    public int rowSize = 3;          // x축 (가로) 개수
    public int colSize = 3;          // z축 (앞뒤) 개수

    private int currentCount = 0;
    private List<GameObject> moneyInstances = new List<GameObject>();

    public Transform UnlockZone;

    private void Reset()
    {
        // 트리거 자동 설정
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }
    private Coroutine absorbCoroutine;

    private PlayerWallet currentPlayerWallet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (absorbCoroutine != null)
            {
                StopCoroutine(absorbCoroutine);
            }

            currentPlayerWallet = other.GetComponent<PlayerWallet>();

            if (currentPlayerWallet != null)
            {
                Debug.Log("[MoneyStacker] 플레이어 감지 → 돈 흡수 시작");

                absorbCoroutine = StartCoroutine(AbsorbAllMoney(currentPlayerWallet));
            }
        }
    }

    // 플레이어가 영역에서 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (absorbCoroutine != null)
            {
                Debug.Log("[MoneyStacker] 플레이어 이탈 → 돈 흡수 중단");

                StopCoroutine(absorbCoroutine);

                absorbCoroutine = null;
                currentPlayerWallet = null;
            }
        }
    }

    private IEnumerator AbsorbAllMoney(PlayerWallet wallet)
    {
        if (wallet == null) yield break;

        List<Transform> moneyList = new List<Transform>();
        foreach (Transform child in stackPoint)
        {
            if (child != null) moneyList.Add(child);
        }

        // 순서대로 하나씩 흡수
        foreach (var money in moneyList)
        {
            if (money == null) continue;
            getmoneySource.PlayOneShot(getmoneySource.clip);
            StartCoroutine(MoveMoneyToPlayer(money, wallet));
            yield return new WaitForSeconds(0.1f);
        }
        ArrowView.I.HideArrow(stackPoint);  

        ArrowView.I.ShowArrow(UnlockZone, 100);

        currentCount = 0;
        moneyInstances.Clear();
    }

    private IEnumerator MoveMoneyToPlayer(Transform money, PlayerWallet wallet)
    {
        Vector3 start = money.position;
        Vector3 target = wallet.transform.position; 

        float duration = 0.4f;
        float elapsed = 0f;

        // 머리 위로 올라가는 정도
        float heightOffset = 2.5f;

        while (elapsed < duration && wallet != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f); // EaseOut

            float curveY = Mathf.Sin(t * Mathf.PI) * heightOffset;

            Vector3 curvedTarget = Vector3.Lerp(start, target, t);
            curvedTarget.y += curveY;

            money.position = curvedTarget;
            yield return null;
        }

        if (wallet != null)
        {
            wallet.AddMoney(moneyValue);
        }

        Destroy(money.gameObject);
    }


    public void AddMoneyForBread(int breadCount)
    {
        ArrowView.I.ShowArrow(stackPoint, 80); 
        int totalMoney = breadCount * 5;
        for (int i = 0; i < totalMoney; i++)
        {
            SpawnOneMoney();
        }
    }

    private void SpawnOneMoney()
    {
        if (moneyPrefab == null || stackPoint == null) return;

        int index = currentCount;

        int x = index % rowSize;
        int z = (index / rowSize) % colSize;
        int y = index / (rowSize * colSize);

        Vector3 targetPos = stackPoint.position
                          + new Vector3(x * xOffset, y * yOffset, z * zOffset);

        // 시작 위치는 살짝 아래
        Vector3 startPos = targetPos + Vector3.down * 0.5f;
        Quaternion rotation = Quaternion.Euler(0f, 90f, 0f);     

        GameObject obj = Instantiate(moneyPrefab, startPos, rotation, stackPoint);

        StartCoroutine(AnimationHelper.SmoothRise_Coroutine(obj.transform, targetPos));

        moneyInstances.Add(obj);
        currentCount++;
        putmoneySource.PlayOneShot(putmoneySource.clip);

        if (currentCount >= maxStack)
        {
            Debug.Log("[MoneyStacker] 최대 스택 도달. 다시 처음부터 시작합니다.");
            currentCount = 0;
        }
    }



    public void RemoveVisualMoney(int amount)
    {
        amount = Mathf.Min(amount, moneyInstances.Count);

        for (int i = 0; i < amount; i++)
        {
            var last = moneyInstances[moneyInstances.Count - 1];
            moneyInstances.RemoveAt(moneyInstances.Count - 1);
            Destroy(last);
            currentCount--;
        }
    }
}
