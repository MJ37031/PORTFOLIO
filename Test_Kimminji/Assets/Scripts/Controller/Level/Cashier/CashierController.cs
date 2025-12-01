// CashierController.cs

using System.Collections;
using UnityEngine;

public class CashierController : MonoBehaviour
{
    public CashierQueueManager queueManager;
    public GameObject bagPrefab;      // 봉투 프리팹 (Inspector에서 설정)
    public Transform bagSpawnPoint;   // 봉투가 생성될 위치 (Inspector에서 설정)

    private bool isPlayerPresent = false;
    private bool isBusy = false;
    private GameObject currentBagInstance; // 현재 생성된 봉투 인스턴스

    private void Update()
    {
        // Update에서는 코루틴을 "시작" 시키는 역할만 담당
        if (isPlayerPresent && !isBusy && queueManager.CustomerCount > 0)
        {
            CustomerController customerToServe = queueManager.GetFirstInQueue();
            if (customerToServe != null)
            {
                // "손님 응대" 절차 시작
                StartCoroutine(ServeCustomerSequence(customerToServe));
            }
        }
    }

    private IEnumerator ServeCustomerSequence(CustomerController customer)
    {
        isBusy = true; // "나 이제 바쁨"

        // 1. 봉투 생성
        currentBagInstance = Instantiate(bagPrefab, bagSpawnPoint.position, bagSpawnPoint.rotation);

        // 2. 손님에게 빵을 포장하라고 명령하고, "끝날 때까지 기다리기"
        yield return StartCoroutine(customer.PackBreadsIntoBag(currentBagInstance));

        // 3. 빵 포장이 끝나면, 계산 시작 명령
        customer.StartPayment(this);

        // isBusy = false; 는 OnPaymentComplete에서 처리되므로 여기선 필요 없음
    }


    // 이 함수를 호출하면 계산이 끝났다는 신호
    public void OnPaymentComplete(CustomerController completedCustomer)
    {
        isBusy = false;
        currentBagInstance = null; // 봉투는 떠나는 손님이 가져갔으므로 참조를 비움
    }

    // 플레이어가 계산대 트리거에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerPresent = true;
        }
    }

    // 플레이어가 계산대 트리거에서 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerPresent = false;
            // 플레이어가 떠나면 생성되어 있던 봉투는 제거 (선택적)
            if (currentBagInstance != null && !isBusy)
            {
                Destroy(currentBagInstance);
                currentBagInstance = null;
            }
        }
    }
}