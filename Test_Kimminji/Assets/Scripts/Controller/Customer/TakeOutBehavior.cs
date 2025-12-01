using UnityEngine;

public class TakeOutBehavior : ICustomerBehavior
{
    public void Enter(CustomerController customer)
    {
        // ================== 디버그 코드 추가 시작 ==================
        Debug.Log("--- 변수 체크 시작 ---");
        Debug.Log("customer: " + (customer != null ? "있음" : "없음(null)"));
        Debug.Log("customer.view: " + (customer.view != null ? "있음" : "없음(null)"));
        Debug.Log("customer.targetCounter: " + (customer.targetCounter != null ? "있음" : "없음(null)"));
        Debug.Log("customer.data: " + (customer.data != null ? "있음" : "없음(null)"));
        Debug.Log("--- 변수 체크 끝 ---");
        // ================== 디버그 코드 추가 끝 ==================

        // 빠져있던 UI 표시 기능 추가!
        customer.view.ShowDesiredItem(customer.targetCounter.breadData, customer.data.buyAmount);

        customer.agent.SetDestination(customer.myStandPoint.position);
        customer.SetState(CustomerState.WalkingToStand);
    }

    public void OnArriveAtDestination(CustomerController customer)
    {
        // 진열대 도착 시 구매 시작
        if (customer.currentState == CustomerState.WalkingToStand)
        {
            customer.StartCoroutine(customer.BuyRoutine());
        }
        // 계산대 줄 도착 시 대기
        else if (customer.currentState == CustomerState.WalkingToCashier)
        {
            customer.SetState(CustomerState.WaitingToPay);
        }
    }

    public void DecideNextActionAfterBuying(CustomerController customer)
    {
        Debug.Log("[TakeOutBehavior] 빵 구매 완료. 계산대로 이동합니다.");
        customer.targetCounter.ReleaseStandPoint(customer.GetStandIndex());
        customer.GoToCashier(); 
    }
}