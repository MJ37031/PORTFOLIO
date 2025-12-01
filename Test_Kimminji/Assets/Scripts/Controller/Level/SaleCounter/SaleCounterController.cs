using UnityEngine;

public class SaleCounterController : MonoBehaviour
{
    [Header("Data")]
    public BreadData breadData;     // 어떤 빵인지
    public int currentCount;        // 현재 전시된 빵 개수
    public int maxCapacity = 8;
    public int pricePerBread = 5;

    public int maxBread = 8;

    [Header("References")]
    public SaleCounterView view;    // View 연결 (빵 전시대)

    [Header("Stand Points")]
    public Transform[] standPoints;       // 인스펙터에서 3개 등록
    private bool[] occupied;              // 자리 점유 여부

    [Header("References")]
    [Tooltip("손님 스폰을 요청할 스포너")]
    public CustomerSpawner spawner;

    private void Awake()
    {
        occupied = new bool[standPoints.Length];
    }

    public bool HasEmptyStandPoint()
    {
        foreach (bool occ in occupied)
            if (!occ) return true;
        return false;
    }

    public Transform AssignStandPoint(CustomerController customer)
    {
        for (int i = 0; i < standPoints.Length; i++)
        {
            if (!occupied[i])
            {
                occupied[i] = true;
                customer.SetStandIndex(i);
                Debug.Log("[SaleCounter] 자리 배정: " + i);
                return standPoints[i];
            }
        }
        Debug.Log("[SaleCounter] 빈 자리 없음!");
        return null;
    }

    public void ReleaseStandPoint(int index)
    {
        // 유효한 자리 번호인지 확인
        if (index >= 0 && index < occupied.Length)
        {
            if (occupied[index])
            {
                occupied[index] = false;
                Debug.Log($"[SaleCounter] {index}번 자리 비움! 다음 손님 스폰 요청.");

                // 스포너 참조가 연결되어 있다면, 다음 손님 스폰을 요청!
                if (spawner != null)
                {
                    spawner.SpawnNextCustomer(index);
                }
                else
                {
                    Debug.LogWarning("SaleCounter에 CustomerSpawner가 연결되지 않았습니다!");
                }
            }
        }
    }




// 플레이어가 빵을 올려놓을 때
public void AddBread(GameObject bread, int index)
    {
        if (currentCount >= maxBread)
        {
            Debug.Log("판매대가 가득 찼습니다!");
            return;
        }

        currentCount++;
        if (view != null)
            view.PlaceBread(bread, index); // View가 빵을 전시대 위에 배치
    }

    // 손님이 빵 하나 집어갈 때 → 실제 오브젝트 반환
    public GameObject TakeOneBread()
    {
        if (currentCount <= 0) return null;

        GameObject bread = view.TakeLastBread(); // View에서 마지막 빵 반환
        if (bread != null)
        {
            currentCount--;
            Debug.Log("판매대에서 빵 꺼내옴");
        }
        return bread;
    }

    public bool HasBread(int amount = 1)
    {
        return currentCount >= amount;
    }

    public int GetPrice() => pricePerBread;
}
