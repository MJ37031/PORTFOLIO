// EatingZoneManager.cs

using UnityEngine;
using System.Collections.Generic;

public class EatingZoneManager : MonoBehaviour
{
    [Tooltip("손님이 앉을 의자 위치 목록")]
    public List<Transform> seats; // 의자 목록

    // 각 의자가 사용 중인지 상태를 저장하는 리스트
    private List<bool> isSeatOccupied;

    // 자리를 기다리는 손님들의 대기열
    private Queue<CustomerController> waitingCustomers = new Queue<CustomerController>();

    private void Awake()
    {
        // 의자 개수만큼 사용 여부 리스트를 만들고, 모두 '비어있음(false)'으로 초기화
        isSeatOccupied = new List<bool>();
        for (int i = 0; i < seats.Count; i++)
        {
            isSeatOccupied.Add(false);
        }
    }

    private void Update()
    {
        // 대기 중인 손님이 있을 때만 실행
        if (waitingCustomers.Count > 0)
        {
            TryAssignSeat();
        }
    }

    // 손님이 자리를 요청할 때 호출하는 함수
    public void RequestSeat(CustomerController customer)
    {
        Debug.Log($"[EatingZone] {customer.name}가 자리를 요청합니다.");
        waitingCustomers.Enqueue(customer);
        TryAssignSeat(); // 즉시 빈 자리가 있는지 확인
    }

    // 손님이 자리를 비울 때 호출하는 함수
    public void ReleaseSeat(Transform seat)
    {
        int index = seats.IndexOf(seat);
        if (index != -1)
        {
            isSeatOccupied[index] = false;
            Debug.Log($"[EatingZone] {index}번 자리가 비었습니다.");
            TryAssignSeat(); // 자리가 비었으니, 기다리는 다음 손님이 있는지 확인
        }
    }

    // 빈 자리를 찾아 대기 중인 손님에게 배정하는 함수
    private void TryAssignSeat()
    {
        // 기다리는 손님이 있고, 빈 자리가 있을 때
        if (waitingCustomers.Count > 0)
        {
            for (int i = 0; i < seats.Count; i++)
            {
                if (!isSeatOccupied[i])
                {
                    if (seats[i].gameObject.activeInHierarchy)
                    {
                        isSeatOccupied[i] = true;
                        CustomerController customerToSeat = waitingCustomers.Dequeue();

                        Debug.LogAssertion($"[EatingZone] 활성화된 {i}번 자리를 {customerToSeat.name}에게 배정합니다!");
                        customerToSeat.OnSeatAssigned(seats[i]);
                        return;
                    }
                }
            }
            Debug.Log("[EatingZone] 기다리는 손님은 있지만, 빈 자리가 없습니다.");
        }
    }
}