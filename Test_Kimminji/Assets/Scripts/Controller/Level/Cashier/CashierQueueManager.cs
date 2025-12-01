using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CashierQueueManager : MonoBehaviour
{
    [Header("Queue Points")]
    public Transform[] queuePoints;              // 줄 서는 위치들(앞에서부터 순서대로 배치)

    [Header("Envelope Spawn")]
    public GameObject envelopePrefab;            // 봉투 프리팹
    public Transform envelopeSpawnPoint;         // 봉투가 처음 생기는 위치(없으면 손님 손 위치 사용)

    public AudioSource putbreadsound;

    private readonly Queue<CustomerController> waitingCustomers = new Queue<CustomerController>();
    private bool envelopeSpawned = false;
    private bool isPlayerAtCashier = false;

    public int CustomerCount => waitingCustomers.Count;


    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;           // 이 오브젝트는 '플레이어 계산 위치' 트리거로 사용
    }

    // 손님이 줄에 들어올 때 호출
    public int Enqueue(CustomerController customer)
    {
        waitingCustomers.Enqueue(customer);
        Debug.Log("[CashierQueueManager] Enqueue 호출됨. 현재 대기열: " + waitingCustomers.Count);

        // 맨 앞 손님인데 플레이어가 이미 계산 위치에 서 있다면 → 봉투 생성
        if (!envelopeSpawned && waitingCustomers.Count == 1 && isPlayerAtCashier)
        {
            TrySpawnEnvelopeForFrontCustomer();
        }

        return waitingCustomers.Count - 1;
    }

    // 계산 끝난 손님이 빠질 때 호출
    public void Dequeue()
    {
        if (waitingCustomers.Count > 0)
        {
            CustomerController customer = waitingCustomers.Dequeue();
            Debug.Log("[CashierQueueManager] 손님 퇴장 명령. 남은 대기열: " + waitingCustomers.Count);

            // 남은 손님들 앞으로 당기기
            int index = 0;
            foreach (var cust in waitingCustomers)
            {
                if (index < queuePoints.Length)
                {
                    Debug.Log("한 칸 앞으로");
                    cust.MoveToQueuePoint(queuePoints[index].position);
                    index++;
                }
            }

            envelopeSpawned = false; 
            Debug.Log("[QueueManager] 손님 빠짐 → 봉투 리셋");

            if (waitingCustomers.Count > 0 && isPlayerAtCashier)
            {
                TrySpawnEnvelopeForFrontCustomer();
            }
        }
    }



    public CustomerController GetFirstInQueue()
    {
        if (waitingCustomers.Count > 0)
        {
            return waitingCustomers.Peek();
        }
        return null;
    }


    public Vector3 GetQueuePosition(int index)
    {
        if (queuePoints == null || queuePoints.Length == 0)
            return transform.position;

        index = Mathf.Clamp(index, 0, queuePoints.Length - 1);
        return queuePoints[index].position;
    }

    // 플레이어가 계산 위치 트리거에 들어오면 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerAtCashier = true;
            Debug.Log("[CashierQueueManager] 플레이어 계산 위치 도착");

            // 맨 앞 손님이 이미 대기중이라면 봉투 생성
            if (!envelopeSpawned && waitingCustomers.Count > 0)
            {
                TrySpawnEnvelopeForFrontCustomer();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerAtCashier = false;
            Debug.Log("[CashierQueueManager] 플레이어 계산 위치 이탈");

        }
    }


    private void TrySpawnEnvelopeForFrontCustomer()
    {
        Debug.Log($"[TrySpawnEnvelope] 시도, envelopeSpawned={envelopeSpawned}, count={waitingCustomers.Count}, player={isPlayerAtCashier}");

        if (envelopeSpawned) return;
        if (waitingCustomers.Count == 0) return;
        if (envelopeSpawned) return;
        if (waitingCustomers.Count == 0) return;
        if (!isPlayerAtCashier) return; // 플레이어 없으면 봉투 안 줌

        var front = waitingCustomers.Peek();
        if (front == null || front.bagHoldPoint == null || envelopePrefab == null)
        {
            Debug.LogWarning("[TrySpawnEnvelope] 맨 앞 손님이 null");
            return;
        }

        if (front.bagHoldPoint == null || envelopePrefab == null)
        {
            Debug.LogWarning("[TrySpawnEnvelope] 봉투 스폰 실패 (bagHoldPoint 또는 prefab 없음)");
            return;
        }


        // 봉투 생성
        Debug.Log("[TrySpawnEnvelope] 봉투 생성 시작");
        var bag = Object.Instantiate(envelopePrefab, 
            envelopeSpawnPoint.position, 
            envelopeSpawnPoint.rotation * Quaternion.Euler(0, 90, 0));


        // 빵 이동 코루틴 시작 (하나씩 순서대로)
        front.StartCoroutine(MoveBreadsIntoBag(front, bag.transform));

        envelopeSpawned = true;
        Debug.Log("[CashierQueueManager] 봉투 생성 + 빵 하나씩 이동 시작");
    }

    private IEnumerator MoveBreadsIntoBag(CustomerController customer, Transform bag)
    {
        var breads = new List<GameObject>(customer.GetCarriedBreads());
        customer.ClearCarriedBreads();

        foreach (var bread in breads)
        {
            if (bread == null) continue;

            Vector3 start = bread.transform.position;
            Vector3 target = bag.position;

            float duration = 0.3f;
            float t = 0f;

            Vector3 control = (start + target) / 2f + Vector3.up * 4f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float lerp = t / duration;

                // EaseOut 적용 (빨려 들어가는 속도감)
                lerp = 1f - Mathf.Pow(1f - lerp, 3f);

                bread.transform.position = Bezier(start, control, target, lerp);
                yield return null;
            }

            // 봉투에 도착하면 파괴
            if (putbreadsound != null)
            {
                putbreadsound.PlayOneShot(putbreadsound.clip);
            }
            Object.Destroy(bread);

            // 다음 빵은 약간 텀 두고
            yield return new WaitForSeconds(0.1f);
        }

        // 모든 빵이 들어가면 봉투 애니메이션 실행
        Animator anim = bag.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Close");
            Debug.Log("[CashierQueueManager] 봉투 닫힘 애니메이션 실행");
        }
        

        bag.transform.SetParent(customer.bagHoldPoint, worldPositionStays: false);
        bag.transform.localPosition = Vector3.zero;
        bag.transform.localRotation = Quaternion.identity;
        Debug.Log("손님 손으로 이동");

        yield return new WaitForSeconds(0.5f);

        customer.SetState(CustomerState.Paying);

        MoneyStacker stacker = FindObjectOfType<MoneyStacker>();

        if (stacker != null)
        {
            stacker.AddMoneyForBread(customer.data.buyAmount);
        }
    }

    // 보조 함수
    private Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }
}
