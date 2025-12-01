// 손님의 행동 로직을 정의하는 인터페이스
public interface ICustomerBehavior
{
    // 이 행동을 시작할 때 호출될 함수
    void Enter(CustomerController customer);

    // 목적지에 도착했을 때 호출될 함수
    void OnArriveAtDestination(CustomerController customer);

    // 빵을 모두 구매했을 때의 다음 행동을 결정하는 함수
    void DecideNextActionAfterBuying(CustomerController customer);
}