using UnityEngine;

public class EatingInBehavior : ICustomerBehavior
{
    public void Enter(CustomerController customer)
    {
        
        customer.view.ShowDesiredItem(customer.targetCounter.breadData, customer.data.buyAmount);

        customer.agent.SetDestination(customer.myStandPoint.position);
        customer.SetState(CustomerState.WalkingToStand);
    }

    public void DecideNextActionAfterBuying(CustomerController customer)
    {
        Debug.Log("매장 식사 손님: 먹는 공간으로 이동합니다.");
        customer.targetCounter.ReleaseStandPoint(customer.GetStandIndex());

        customer.agent.SetDestination(customer.eatZoneWaitPoint.position);
        customer.SetState(CustomerState.WalkingToEatZone);
    }

    public void OnArriveAtDestination(CustomerController customer)
    {
        switch (customer.currentState)
        {
            case CustomerState.WalkingToStand:
                customer.StartCoroutine(customer.BuyRoutine());
                break;
            case CustomerState.WalkingToEatZone:
                customer.SetState(CustomerState.WaitingInEatZone);
                customer.eatingZoneManager.RequestSeat(customer);
                break;
            case CustomerState.WalkingToSeat:
                customer.StartCoroutine(customer.EatRoutine());
                break;
            case CustomerState.WalkingToTrashCan:
                customer.StartCoroutine(customer.ThrowTrashRoutine());
                break;
            case CustomerState.WalkingToCashier:
                customer.PayImmediatelyAndLeave();
                break;
        }
    }
}